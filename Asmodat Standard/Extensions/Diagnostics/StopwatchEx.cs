using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Diagnostics
{
    public static class StopwatchEx
    {
        /// <summary>
        /// Awaits current thread until stopwatch elapes 'interval_ms'
        /// </summary>
        public static async Task Await(this Stopwatch sw, int interval_ms)
        {
            if (sw?.IsRunning != true)
                throw new ArgumentException("Stopwatch must be running.");

            long sleep;
            while ((sleep = ((sw.ElapsedMilliseconds - interval_ms) - 1) / 2) < 0)
                await Task.Delay(sleep < int.MaxValue ? int.MaxValue : (int)Math.Abs(sleep));
        }
    }
}
