using AsmodatStandard.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class EnumEx
    {
        public static string[] ToStringFlagArray<T>(this T @enum) where T : Enum
        {
            var iBase = Convert.ToInt32(@enum);
            if (iBase == 0) //don't append zero flags 
                return new string[0];

            var arr = EnumEx.ToArray<T>();

            var result = new List<string>();
            foreach (var e in arr)
            {
                var iCurent = Convert.ToInt32(e);

                if (iCurent < 0)
                    throw new Exception($"ToStringFlagArray, does not accept negative enums, but got: {iCurent}, use flags from range 0 to 1 << 31");

                if (iCurent.GetBitShiftPositions().Length > 1)
                    continue; //don't append combined flags

                if ((iCurent & iBase) != 0)
                    result.Add(e.ToString());
            }

            return result.Distinct().ToArray();
        }

        public static string[] ToStringArray<T>() where T : Enum
            => EnumEx.ToArray<T>().Select(e => e.ToString()).ToArray();

        public static T[] ToArray<T>() where T : Enum
            => Enum.GetValues(typeof(T)).OfType<T>().ToArray();

        public static object[] ToArray(Type type)
            => Enum.GetValues(type).OfType<object>().ToArray();

        public static T ToEnum<T>(this string s) where T : Enum
            => (T)Enum.Parse(typeof(T), s);

        public static T ToEnumOrDefault<T>(this string s, T @default) where T : Enum
        {
            try
            {
                return (T)Enum.Parse(typeof(T), s);
            }
            catch
            {
                return @default;
            }
        }

        /// <summary>
        /// Converts integer to specified enum then calls toString method and returns result, otherwise if fails - default value
        /// </summary>
        public static string ToEnumStringOrDefault<T>(this int e, string @default = null) where T : Enum
        {
            try
            {
                return ((T)(object)e).ToString();
            }
            catch
            {
                return @default;
            }
        }

        public static IEnumerable<T> ToEnum<T>(this IEnumerable<string> arr) where T : Enum
            => arr.Select(s => (T)Enum.Parse(typeof(T), s));

        /*public static string GetDescriptionString<E>(this E val) where E: Enum
        {
            var attributes = GetCustomAttributes<E, DescriptionAttribute>(val);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }*/

        public static A[] GetCustomAttributes<E,A>(this E val) where E : Enum where A : Attribute
            => (A[])GetCustomAttributes(val, typeof(A));

        private static object[] GetCustomAttributes<E>(this E val, Type type) where E : Enum
        {
            var attributes = val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(type, inherit: false);

            return attributes;
        }

        public static object[] GetAttributes<E>(this E val, Type enumType)
        {
            var field = enumType.GetField(val.ToString());
            var attributes = field.GetCustomAttributes(false);

            return attributes;
        }
    }
}
