using System;
using System.Collections.Generic;
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

        public static T ToEnum<T>(this string s) where T : Enum
            => (T)Enum.Parse(typeof(T), s);

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
    }
}
