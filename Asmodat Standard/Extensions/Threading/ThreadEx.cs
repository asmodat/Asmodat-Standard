using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class ThreadEx
    {
        /// <summary>
        /// Joins multiple threads 
        /// </summary>
        public static void Join(this IEnumerable<Thread> threads)
            => threads.ForEach(t => t.Join());
        
        public static void For(int from, int to, Action<int> action)
            => ForEach(Enumerable.Range(from, to - from), action);

        public static void ForEach<K>(IEnumerable<K> source, Action<K> action, ThreadPriority priority = ThreadPriority.Highest)
        {
            var threads = source.Select(k =>
            {
                var t = new Thread(() => action(k));
                t.Priority = priority;
                t.IsBackground = false;
                t.Start();
                return t;
            });

            threads.Join();
        }
    }
}
