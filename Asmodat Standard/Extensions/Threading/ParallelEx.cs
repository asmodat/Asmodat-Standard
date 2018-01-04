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

        public static async Task ForEachAsync<K>(IEnumerable<K> source, Func<K, Task> func)
            => await Task.WhenAll(source.Select(x => func(x)));

        public static async Task ForEachAsync<K>(IEnumerable<K> source, Action<K> action)
            => await Task.WhenAll(source.Select(x => new Task(() => action(x))));
    }
}
