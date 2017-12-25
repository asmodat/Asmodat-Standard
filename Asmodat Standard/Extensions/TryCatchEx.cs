using System;
namespace AsmodatStandard.Extensions
{
    

    public static class TryCatchEx
    {
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

        public static bool Action(this Action action) => action.Action(out Exception ex);

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
