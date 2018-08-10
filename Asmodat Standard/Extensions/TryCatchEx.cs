using System;
using System.Collections.Generic;
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
            var exceptions = new List<Exception>();
            do
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            } while (--maxRepeats > 0);

            throw new AggregateException(exceptions);
        }

        public static async Task<T> FuncRepeatAsync<T>(this Func<Task<T>> func, int maxRepeats = 1)
        {
            var exceptions = new List<Exception>();
            do
            {
                try
                {
                    return await func();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            } while (--maxRepeats > 0);

            throw new AggregateException(exceptions);
        }


        public static bool Action(this Action action) => action.Action(out Exception ex);
        public static void ActionRepeat(this Action action, int maxRepeats = 1)
        {
            var exceptions = new List<Exception>();
            do
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            } while (--maxRepeats > 0);

            throw new AggregateException(exceptions);
        }


        public static bool Action(this Action action, out Exception exception)
        {
            try
            {
                exception = null;
                action();
                return true;
            }
            catch(Exception ex)
            {
                exception = ex;
                return false;
            }
        }

    }
}
