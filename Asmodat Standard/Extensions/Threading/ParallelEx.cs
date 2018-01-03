using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class ParallelEx
    {
        public static async Task ForAsync(int from, int to, Func<int, Task> func)
            => await Task.WhenAll(Enumerable.Range(from, to - from).Select(i => func(i)));
    }
}
