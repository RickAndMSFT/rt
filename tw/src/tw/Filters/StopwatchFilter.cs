using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Logging;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using MvcSample.Services;

namespace MvcSample.Filters
{
    public class StopwatchFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly StopwatchOptions _options;
        private readonly IActionTimer _timer;

        public StopwatchFilter(
            ILoggerFactory loggerFactory, 
            IOptions<StopwatchOptions> options,
            IActionTimer timer)
        {
            _logger = loggerFactory.Create<UseStopwatchAttribute>();
            _options = options.Options;
            _timer = timer;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _timer.Timer.Start();
        }

        // The OnResultExecuting runs right before the view, and is the last chance to record data.
        // 
        // Set ShowTimeInViews = false to skip the output.
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResult;
            if (_options.ShowTimeInViews && viewResult != null)
            {
                // This will be read by the layout page.
                viewResult.ViewData.Add("elapsedTime", _timer.Timer.Elapsed.ToString("ss\\.fff"));
            }
        }

        // OnResultExecuted runs right after the view, and so we can't change the content.
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            var timingValues = new ActionTimingLogValues()
            {
                ElapsedTime = _timer.Timer.Elapsed,
                RouteValues = filterContext.RouteData.Values,
                QueryString = filterContext.HttpContext.Request.QueryString.ToString(),
            };

            LogLevel logLevel;
            if (timingValues.ElapsedTime.TotalSeconds > _options.ErrorTime)
            {
                logLevel = LogLevel.Error;
            }
            else if (timingValues.ElapsedTime.TotalSeconds > _options.WarningTime)
            {
                logLevel = LogLevel.Warning;
            }
            else if (timingValues.ElapsedTime.TotalSeconds > _options.InformationTime)
            {
                logLevel = LogLevel.Information;
            }
            else
            {
                logLevel = LogLevel.Verbose;
            }

            _logger.Write(logLevel, eventId: 0, state: timingValues, exception: null, formatter: null);
        }

        private class ActionTimingLogValues : LoggerStructureBase
        {
            public TimeSpan ElapsedTime { get; set; }

            public IDictionary<string, object> RouteValues { get; set; }

            public string QueryString { get; set; }

            public override string Format()
            {
                return LogFormatter.Formatter(this, null);
            }
        }
    }
}