using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class ParallelEx
    {
        public static async Task ForAsync(int from, int to, Action<int> action)
            => await Task.WhenAll(Enumerable.Range(from, to - from).Select(i => new Task(() => action(i))));

        public static async Task ForAsync(int from, int to, Func<int, Task> func)
            => await Task.WhenAll(Enumerable.Range(from, to - from).Select(i => func(i)));

        public static async Task ForEachAsync<K>(IEnumerable<K> source, Func<K, Task> func, int maxDegreeOfParallelism = 0)
        {
            var tasks = new List<Task>();
            List<Task> pending = null;

            foreach (var x in source)
            {
                if (maxDegreeOfParallelism > 0)
                    pending = tasks.Where(t => t != null && !t.IsCanceled && !t.IsCompleted && !t.IsFaulted)?.ToList() ?? new List<Task>();

                if (maxDegreeOfParallelism <= 0 || pending.Count < maxDegreeOfParallelism)
                {
                    tasks.Add(func(x));
                    continue;
                }

                if (!pending.IsNullOrEmpty())
                    await Task.WhenAny(pending);
            }

            if (!tasks.IsNullOrEmpty())
                await Task.WhenAll(tasks);
        }

        public static async Task ForEachAsync<K>(IEnumerable<K> source, Action<K> action)
            => await Task.WhenAll(source.Select(x => new Task(() => action(x))));
    }
}
