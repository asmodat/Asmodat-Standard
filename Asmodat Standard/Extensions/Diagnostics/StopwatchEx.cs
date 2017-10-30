using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AsmodatStandard.Extensions.Diagnostics
{
    public static class StopwatchEx
    {
        /// <summary>
        /// Awaits current thread until stopwatch elapes 'interval_ms'
        /// </summary>
        public static void Await(this Stopwatch sw, int interval_ms)
        {
            if (sw?.IsRunning != true)
                throw new ArgumentException("Stopwatch must be running.");

            long sleep;
            while ((sleep = ((sw.ElapsedMilliseconds - interval_ms) - 1)/2) < 0)
                Thread.Sleep(sleep < int.MaxValue ? int.MaxValue : (int)Math.Abs(sleep));
        }
    }
}
