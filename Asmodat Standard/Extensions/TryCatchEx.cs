using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions
{
    public static class TryCatchEx
    {
        public static Task<Exception> CatchExceptionAsync(this Task task, bool catchDisable = false)
            => task.CatchExceptionAsync<Exception>(catchDisable: catchDisable);

        public static async Task<E> CatchExceptionAsync<E>(this Task task, bool catchDisable = false) where E : Exception
        {
            if (catchDisable)
            {
                await task;
                return null;
            }

            try
            {
                await task;
                return null;
            }
            catch (E exception)
            {
                return exception;
            }
        }

        public static T? FuncNullable<T>(this Func<T> func, T? @default = null) where T : struct
            => (func.Func(out T val, out Exception ex, @default ?? default(T))) ? val : @default;

        /// <summary>
        /// Try-catch oneliner block
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_func">Function to be executed</param>
        /// <param name="_default">Default value to be returned if funcion throws exception</param>
        /// <returns></returns>
        public static T Func<T>(this Func<T> func, T @default = default(T))
        {
            func.Func(out T val, out Exception ex, @default);
            return val;
        }

        /// <summary>
        /// Try-Cach Fun + return Value and Exception
        /// </summary>
        public static T Func<T>(this Func<T> func, out Exception exception, T @default = default(T))
        {
            func.Func(out T val, out Exception ex, @default);
            exception = ex;
            return val;
        }

        public static bool Func<T>(this Func<T> func, out T value, out Exception exception, T @default = default(T))
        {
            try
            {
                exception = null;
                value = func();
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                value = @default;
                return false;
            }
        }

        public static T FuncRepeat<T>(this Func<T> func, int maxRepeats = 1)
        {
            int count = 0;
            var sw = Stopwatch.StartNew();
            ExceptionDispatchInfo exception = null;
            do
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    ex.Data?.Add($"UserDefined_{GuidEx.SlimUID()}", $"TryCatchEx() => Count: {++count}, Time: {DateTime.UtcNow.ToLongDateTimeString()}, Elapsed: {sw.ElapsedMilliseconds} [ms]");
                    exception = ExceptionDispatchInfo.Capture(ex);
                }
            } while (--maxRepeats > 0);

            exception.Throw();
            throw exception.SourceException;
        }

        public static Task<T> FuncRepeatAsync<T>(this Func<Task<T>> func, int maxRepeats = 1, int delay = 1000)
            => FuncRepeatAsync<T, Exception>(func, maxRepeats: maxRepeats, delay: delay);

        public static async Task<T> FuncRepeatAsync<T, E>(this Func<Task<T>> func, int maxRepeats, int delay) where E : Exception
        {
            int count = 0;
            var sw = Stopwatch.StartNew();
            ExceptionDispatchInfo exception = null;
            do
            {
                try
                {
                    return await func();
                }
                catch (E ex)
                {
                    ex.Data?.Add($"UserDefined_{GuidEx.SlimUID()}", $"TryCatchEx() => Count: {++count}, Time: {DateTime.UtcNow.ToLongDateTimeString()}, Elapsed: {sw.ElapsedMilliseconds} [ms]");
                    exception = ExceptionDispatchInfo.Capture(ex);

                    if ((maxRepeats - 1) > 0)
                        await Task.Delay(delay);
                }
            } while (--maxRepeats > 0);

            exception.Throw();
            throw exception.SourceException;
        }

        public static Task TryCatchRetryAsync(this Task task, int maxRepeats = 1, int delay = 1000)
            => TryCatchRetryAsync<Exception>(task, maxRepeats: maxRepeats, delay: delay);

        public static async Task TryCatchRetryAsync<E>(this Task task, int maxRepeats, int delay) where E : Exception
        {
            int count = 0;
            var sw = Stopwatch.StartNew();
            ExceptionDispatchInfo exception = null;
            do
            {
                try
                {
                    await task;
                    return;
                }
                catch (E ex)
                {
                    ex.Data?.Add($"UserDefined_{GuidEx.SlimUID()}", $"TryCatchEx() => Count: {++count}, Time: {DateTime.UtcNow.ToLongDateTimeString()}, Elapsed: {sw.ElapsedMilliseconds} [ms]");
                    exception = ExceptionDispatchInfo.Capture(ex);

                    if ((maxRepeats - 1) > 0)
                        await Task.Delay(delay);
                }
            } while (--maxRepeats > 0);

            exception.Throw();
            throw exception.SourceException;
        }

        public static Task<T> TryCatchRetryAsync<T>(this Task<T> task, int maxRepeats = 1, int delay = 1000, int delayIncrement = 10, int timeout_ms = int.MaxValue)
            => TryCatchRetryAsync<T, Exception>(task, maxRepeats: maxRepeats, delay: delay, delayIncrement: delayIncrement, timeout_ms: timeout_ms);

        public static async Task<T> TryCatchRetryAsync<T, E>(this Task<T> task, 
            int maxRepeats, 
            int delay,
            int delayIncrement,
            int timeout_ms) where E : Exception
        {
            int count = 0;
            var sw = Stopwatch.StartNew();
            ExceptionDispatchInfo exception = null;
            do
            {
                try
                {
                    return await task;
                }
                catch (E ex)
                {
                    ex.Data?.Add($"UserDefined_{GuidEx.SlimUID()}", $"TryCatchEx() => Count: {++count}, Time: {DateTime.UtcNow.ToLongDateTimeString()}, Elapsed: {sw.ElapsedMilliseconds}/{timeout_ms} [ms]");
                    exception = ExceptionDispatchInfo.Capture(ex);

                    if ((maxRepeats - 1) > 0)
                    {
                        await Task.Delay(delay);
                        delay += delayIncrement;
                    }

                    if (sw.ElapsedMilliseconds > timeout_ms)
                    {
                        exception.Throw();
                        throw exception.SourceException;
                    }
                }
            } while (--maxRepeats > 0);

            exception.Throw();
            throw exception.SourceException;
        }

        public static bool Action(this Action action) => action.Action(out ExceptionDispatchInfo ex);
        public static void ActionRepeat(this Action action, int maxRepeats = 1, int onErrorAwait_ms = 1000)
        {
            int count = 0;
            var sw = Stopwatch.StartNew();
            ExceptionDispatchInfo exception;
            do
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    ex.Data?.Add($"UserDefined_{GuidEx.SlimUID()}", $"TryCatchEx() => Count: {++count}, Time: {DateTime.UtcNow.ToLongDateTimeString()}, Elapsed: {sw.ElapsedMilliseconds} [ms]");
                    exception = ExceptionDispatchInfo.Capture(ex);

                    if (onErrorAwait_ms > 0 && (maxRepeats - 1) > 0)
                        Thread.Sleep(onErrorAwait_ms);
                }
            } while (--maxRepeats > 0);

            exception.Throw();
            throw exception.SourceException;
        }


        public static bool Action(this Action action, out ExceptionDispatchInfo exception)
        {
            try
            {
                exception = null;
                action();
                return true;
            }
            catch(Exception ex)
            {
                exception = ExceptionDispatchInfo.Capture(ex);
                return false;
            }
        }

    }
}
