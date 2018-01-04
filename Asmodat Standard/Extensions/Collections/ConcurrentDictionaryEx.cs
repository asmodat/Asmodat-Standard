using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Collections
{
    public static class ConcurrentDictionaryEx
    {
        public static Dictionary<K,V> ToDictionary<K,V>(this ConcurrentDictionary<K, V> dict)
            => dict.ToDictionary(x => x.Key, x => x.Value);

        public static void Add<K,V>(this ConcurrentDictionary<K,V> dict, K key, V value, int timeout = int.MaxValue)
        {
            var sw = Stopwatch.StartNew();
            while(!dict.TryAdd(key, value))
            {
                if (sw.ElapsedMilliseconds > timeout)
                    throw new TimeoutException();

                Thread.Sleep(1);
            }
        }

        public static async Task AddAsync<K, V>(this ConcurrentDictionary<K, V> dict, K key, V value, int timeout = int.MaxValue)
        {
            var sw = Stopwatch.StartNew();
            while (!dict.TryAdd(key, value))
            {
                if (sw.ElapsedMilliseconds > timeout)
                    throw new TimeoutException();

                await Task.Delay(1);
            }
        }

        public static V GetValue<K, V>(this ConcurrentDictionary<K, V> dict, K key, int timeout = int.MaxValue)
        {
            var sw = Stopwatch.StartNew();
            V value;
            while (!dict.TryGetValue(key, out value))
            {
                if (sw.ElapsedMilliseconds > timeout)
                    throw new TimeoutException();

                Thread.Sleep(1);
            }

            return value;
        }

        public static async Task<V> GetValueAsync<K, V>(this ConcurrentDictionary<K, V> dict, K key, int timeout = int.MaxValue)
        {
            var sw = Stopwatch.StartNew();
            V value;
            while (!dict.TryGetValue(key, out value))
            {
                if (sw.ElapsedMilliseconds > timeout)
                    throw new TimeoutException();

                await Task.Delay(1);
            }

            return value;
        }
    }
}
