using System.Diagnostics;

namespace MvcSample.Services
{
    public class ActionTimer : IActionTimer
    {
        public ActionTimer()
        {
            Timer = new Stopwatch();
        }

        public Stopwatch Timer { get; }
    }
}