using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Types
{
    public class RateLimit
    {
        /// <summary>
        /// Hits/Entries per second
        /// </summary>
        private double span;
        private Stopwatch sw;
        private SemaphoreSlim ss;

        public RateLimit(double hitsPerSecond)
        {
            sw = Stopwatch.StartNew();

            if (hitsPerSecond <= 0 || hitsPerSecond.IsInfinity() || hitsPerSecond.IsNaN())
                throw new Exception($"hitsPerSecond can't be <= 0, infinity or NaN, but was: '{hitsPerSecond}'");

            span = (int)Math.Min((double)1000 / hitsPerSecond, int.MaxValue);
            ss = new SemaphoreSlim(1, 1);
        }

        public async Task<bool> Enter(CancellationToken ct = default(CancellationToken))
        {
            return await ss.Lock(async () => {
                while(span - sw.ElapsedMilliseconds > 0)
                {
                    if (ct.IsCancellationRequested == true)
                        return false;

                    var delay = (int)Math.Min(Math.Max(span - sw.ElapsedMilliseconds, 1), 100);
                    await Task.Delay(delay);
                }

                return true;
            });
        }
    }
}
