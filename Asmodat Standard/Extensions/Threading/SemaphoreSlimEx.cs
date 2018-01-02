using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class SemaphoreSlimEx
    {
        public static T Run<T>(this SemaphoreSlim ss, Func<T> func)
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

        public static void Run(this SemaphoreSlim ss, Action action)
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

        public static async Task<T> RunAsync<T>(this SemaphoreSlim ss, Func<T> func)
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

        public static async Task RunAsync(this SemaphoreSlim ss, Action action)
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

        public static async Task Run<T>(this SemaphoreSlim ss, Func<Task> func)
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

        public static async Task<T> Run<T>(this SemaphoreSlim ss, Func<Task<T>> func)
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
