using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace MvcSample.Filters
{
    public class DelayAttribute : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        public DelayAttribute(int randomDelay, int fixedDelay = 0)
        {
            DelayRandom = randomDelay;
            DelayFixed = fixedDelay;
            Rand = new Random();
        }

        public int Order { get; set; }

        public int DelayRandom { get; private set; }
        public int DelayFixed { get; private set; }
        public Random Rand { get; private set; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Method == "GET")
            {
                // slow down incoming GET requests
                await Task.Delay(Rand.Next(0, DelayRandom));
                await Task.Delay(DelayFixed);
            }

            var executedContext = await next();

            if (executedContext.Result is ViewResult)
            {
                // slow down outgoing view results
                await Task.Delay(Rand.Next(0, DelayRandom));
            }
        }
    }
}