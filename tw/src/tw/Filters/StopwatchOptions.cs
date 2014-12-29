
namespace MvcSample.Filters
{
    public class StopwatchOptions
    {
        public double ErrorTime{ get; set; }

        public double WarningTime { get; set; }

        public double InformationTime { get; set; }

        public bool ShowTimeInViews { get; set; }
    }
}