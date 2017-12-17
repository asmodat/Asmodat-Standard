using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Threading
{
    public static class SemaphoreSlimEx
    {
        public static async Task<T> Run<T>(this SemaphoreSlim ss, Func<T> func)
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

        public static async Task Run(this SemaphoreSlim ss, Action action)
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
