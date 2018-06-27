using System;
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
    }
}
