using System;
using System.Diagnostics;
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

            int sleep = (int)(interval_ms - sw.ElapsedMilliseconds - 1);

            if (sleep <= 0)
                return;

            await Task.Delay(sleep);
        }
    }
}
