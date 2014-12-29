using Microsoft.AspNet.Mvc;

namespace MvcSample.Filters
{
    public class UseStopwatchAttribute : TypeFilterAttribute
    {
        public UseStopwatchAttribute()
            : base(typeof(StopwatchFilter))
        {
        }
    }
}