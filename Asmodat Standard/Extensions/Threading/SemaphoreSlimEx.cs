using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Extensions.Threading
{
    public static class SemaphoreSlimEx
    {
        public static bool IsLocked(this SemaphoreSlim ss)
        {
            if (ss.Wait(0))
            {
                ss.Release();
                return false;
            }

            return true;
        }

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

        public static async Task<T> MultiLockAsync<T>(this IEnumerable<SemaphoreSlim> locks, Task<T> task)
        {
            if (locks == null)
                throw new ArgumentException("Locks can't be undefined.");

            var locked = new List<SemaphoreSlim>();
            try
            {
                foreach (var @lock in locks)
                {
                    try
                    {
                        await @lock.WaitAsync();
                    }
                    finally
                    {
                        locked.Add(@lock);
                    }
                }

                return await task;
            }
            finally
            {
                foreach (var @lock in locked)
                    @lock.Release();
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

        public static async Task MultiLockAsync(this IEnumerable<SemaphoreSlim> locks, Func<Task> func)
        {
            if (locks == null)
                throw new ArgumentException("Locks can't be undefined.");

            var locked = new List<SemaphoreSlim>();
            try
            {
                foreach (var @lock in locks)
                {
                    try
                    {
                        await @lock.WaitAsync();
                    }
                    finally
                    {
                        locked.Add(@lock);
                    }
                }

                await func();
            }
            finally
            {
                foreach (var @lock in locked)
                    @lock.Release();
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

        public static async Task<T> MultiLockAsync<T>(this IEnumerable<SemaphoreSlim> locks, Func<Task<T>> func)
        {
            if (locks == null)
                throw new ArgumentException("Locks can't be undefined.");

            var locked = new List<SemaphoreSlim>();
            try
            {
                foreach (var @lock in locks)
                {
                    try
                    {
                        await @lock.WaitAsync();
                    }
                    finally
                    {
                        locked.Add(@lock);
                    }
                }

                return await func();
            }
            finally
            {
                foreach (var @lock in locked)
                    @lock.Release();
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
