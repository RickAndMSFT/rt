using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using System;
using System.Diagnostics;
using System.Linq;
using tw.Controllers;

// Add your using to bring in your controller


/* Add .AddJsonFile(@"App_Data\configTrace.json")
and 
AppSettings.Configuration = Configuration;

    to startup class.
 
public Startup(IHostingEnvironment env)
    {
      Configuration = new Configuration()
          .AddJsonFile("config.json")
          .AddJsonFile(@"App_Data\configTrace.json") // <-- Add this
          .AddEnvironmentVariables();

      AppSettings.Configuration = Configuration;     // <-- Add this
    }
*/
namespace MvcSample.Filters
{
  public class UseStopwatchAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var controller = filterContext.Controller as HomeController;

      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();


      controller.ViewData["stopWatch"] = stopWatch;
      controller.ViewBag.stopWatch = stopWatch;
    }

    // This is our last chance to log timing info to a view. You must set 
    // SkipTimeInLayout > 0  to skip the output.

    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      var controller = filterContext.Controller as HomeController;


      if (AppSettings.SkipTimeInLayout() > 0)
      {
        return;
      }

      Stopwatch stopWatch = (Stopwatch)controller.ViewBag.stopWatch;

      double et = stopWatch.Elapsed.Seconds +
         (stopWatch.Elapsed.Milliseconds / 1000.0);

      controller.ViewBag.elapsedTime = "Elapsed time: " +
           et.ToString();
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      var controller = filterContext.Controller as HomeController;


      Stopwatch stopWatch = (Stopwatch)controller.ViewBag.stopWatch;
      stopWatch.Stop();

      double et = stopWatch.Elapsed.Seconds +
         (stopWatch.Elapsed.Milliseconds / 1000.0);

      string msg = "ET: " + et.ToString();

      var routeDataKeys = filterContext.RouteData.Values.Keys.ToArray();
      string strKey = string.Empty;

      foreach (var key in routeDataKeys)
      {
        msg += " " + key + ": " + filterContext.RouteData.Values[key];
      }


      var qs = controller.Request.QueryString.ToString();

      if (!String.IsNullOrEmpty(qs))
      {
        msg += " QS: " + qs;
      }

      TraceMessage(et, msg);
    }

    void TraceMessage(double et, string msg)
    {
      if (et > AppSettings.TraceErrorTime())
      {
        Trace.TraceError(msg);
      }
      else if (et > AppSettings.TraceWarningTime())
      {
        Trace.TraceWarning(msg);
      }
      else if (et > AppSettings.TraceInformationTime())
      {
        Trace.TraceInformation(msg);
      }
      else
      {
        Trace.WriteLine(msg);
      }
    }

  }

  public static class AppSettings
  {
    static double dError = 0;
    static double dWarn = 0;
    static double dInfo = 0;
    static int skipLayoutTime = -1;
    public static IConfiguration Configuration { get; set; }

    static double AppStringToDouble(string key)
    {
      return Convert.ToDouble(Configuration.Get(key));
    }

    static int AppStringToInt(string key)
    {
      return Convert.ToInt32(Configuration.Get(key));
    }

    public static double TraceErrorTime()
    {
      if (dError == 0)
        dError = AppStringToDouble("ErrorTime");

      return dError;
    }

    public static double TraceWarningTime()
    {
      if (dWarn == 0)
        dWarn = AppStringToDouble("WarningTime");

      return dWarn;
    }

    public static double TraceInformationTime()
    {
      if (dInfo == 0)
        dInfo = AppStringToDouble("InfoTime");

      return dInfo;
    }

    public static int SkipTimeInLayout()
    {
      if (skipLayoutTime < 0)
        skipLayoutTime = AppStringToInt("SkipTimeInLayout");

      return skipLayoutTime;
    }
  }
}