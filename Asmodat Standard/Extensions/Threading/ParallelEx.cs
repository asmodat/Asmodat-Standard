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
        public static async Task ForAsync(int from, int to, Action<int> action, int maxDegreeOfParallelism = 4)
            => await ParallelEx.ForEachAsync(Enumerable.Range(from, to - from), async i => await new Task(() => action(i)), maxDegreeOfParallelism: maxDegreeOfParallelism);

        public static async Task ForAsync(int from, int to, Func<int, Task> func, int maxDegreeOfParallelism = 4)
            => await ParallelEx.ForEachAsync(Enumerable.Range(from, to - from), async i => await func(i), maxDegreeOfParallelism: maxDegreeOfParallelism);

        public static bool IsAlive(this Task task)
            => task != null && !task.IsCanceled && !task.IsCompleted && !task.IsFaulted;


        public static async Task WhenAnyFinalized(this IEnumerable<Task> tasks, int throttling = 1, int intensity = 10, CancellationTokenSource cts = null)
        {
            if (tasks.IsNullOrEmpty())
                return;

            if (cts == null)
                cts = new CancellationTokenSource();
            do
            {
                foreach(var t in tasks)
                {
                    if (!t.IsAlive())
                    {
                        await t;
                        return;
                    }

                    if(throttling > 0)
                        await Task.Delay(throttling);
                }

                if (intensity > 0)
                    await Task.Delay(intensity);
            } while (!cts.IsCancellationRequested);

            return;
        }

        public static async Task ForEachAsync<K>(IEnumerable<K> source, Func<K, Task> func, int maxDegreeOfParallelism = 4, CancellationTokenSource cts = null)
        {
            var tasks = new List<Task>();
            var pending = new List<Task>();

            foreach (var x in source)
            {
                if (maxDegreeOfParallelism > 0)
                {
                    pending = tasks.Where(t => t.IsAlive())?.ToList() ?? new List<Task>();

                    if(pending.Count >= maxDegreeOfParallelism)
                        await pending.WhenAnyFinalized(throttling: 1, intensity: 10, cts: cts);
                }

                tasks.Add(func(x));
            }

            if (!tasks.IsNullOrEmpty())
                await Task.WhenAll(tasks);
        }

        public static async Task ForEachAsync<K>(IEnumerable<K> source, Action<K> action)
            => await Task.WhenAll(source.Select(x => {
                    var t = new Task(() => action(x));
                    t.Start();
                    return t;
                }));

    }
}
