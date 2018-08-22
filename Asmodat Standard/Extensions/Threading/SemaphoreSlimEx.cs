using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class SemaphoreSlimEx
    {
        public static T Lock<T>(this SemaphoreSlim ss, Func<T> func)
        {
            try
            {
                ss.Wait();
                return func();
            }
            finally
            {
                ss.Release();
            }
        }

        public static void Lock(this SemaphoreSlim ss, Action action)
        {
            try
            {
                ss.Wait();
                action();
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task<T> LockAsync<T>(this SemaphoreSlim ss, Func<T> func)
        {
            try
            {
                await ss.WaitAsync();
                return func();
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task LockAsync(this SemaphoreSlim ss, Action action)
        {
            try
            {
                await ss.WaitAsync();
                action();
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task<T> LockAsync<T>(this SemaphoreSlim ss, Task<T> task)
        {
            try
            {
                await ss.WaitAsync();
                return await task;
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task LockAsync(this SemaphoreSlim ss, Task task)
        {
            try
            {
                await ss.WaitAsync();
                await task;
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task Lock<T>(this SemaphoreSlim ss, Func<Task> func)
        {
            try
            {
                await ss.WaitAsync();
                await func();
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task<T> Lock<T>(this SemaphoreSlim ss, Func<Task<T>> func)
        {
            try
            {
                await ss.WaitAsync();
                return await func();
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task TimeLock<T>(this Stopwatch sw, int rateLimit_ms, SemaphoreSlim ss, Func<Task> func)
        {
            try
            {
                await ss.WaitAsync();
                await func();
            }
            finally
            {
                var left = rateLimit_ms - sw.ElapsedMilliseconds;

                if (left > 0)
                    await Task.Delay((int)left);

                sw.Restart();
                ss.Release();
            }
        }

        public static async Task<T> TimeLock<T>(this Stopwatch sw, int rateLimit_ms, SemaphoreSlim ss, Func<Task<T>> func)
        {
            try
            {
                await ss.WaitAsync();
                return await func();
            }
            finally
            {
                var left = rateLimit_ms - sw.ElapsedMilliseconds;

                if (left > 0)
                    await Task.Delay((int)left);

                sw.Restart();
                ss.Release();
            }
        }
    }
}
