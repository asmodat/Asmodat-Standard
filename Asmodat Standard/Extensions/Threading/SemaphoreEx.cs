using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class SemaphoreEx
    {
        public static Semaphore GetNewOrOpen(string name, int initialCount, int maxCount)
            => Semaphore.TryOpenExisting(name, out var sem) ? sem : new Semaphore(initialCount, maxCount, name);

        public static T Run<T>(this Semaphore s, Func<T> func)
        {
            try
            {
                s.WaitOne();
                return func();
            }
            finally
            {
                s.Release();
            }
        }

        public static void Run(this Semaphore s, Action action)
        {
            try
            {
                s.WaitOne();
                action();
            }
            finally
            {
                s.Release();
            }
        }

        public static async Task Run<T>(this Semaphore s, Func<Task> func)
        {
            try
            {
                s.WaitOne();
                await func();
            }
            finally
            {
                s.Release();
            }
        }

        public static async Task<T> Run<T>(this Semaphore s, Func<Task<T>> func)
        {
            try
            {
                s.WaitOne();
                return await func();
            }
            finally
            {
                s.Release();
            }
        }
    }
}
