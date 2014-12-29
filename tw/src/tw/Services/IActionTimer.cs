using System.Diagnostics;

namespace MvcSample.Services
{
    public interface IActionTimer
    {
        Stopwatch Timer { get; }
    }
}