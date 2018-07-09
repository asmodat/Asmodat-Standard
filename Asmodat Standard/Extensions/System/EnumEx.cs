using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class EnumEx
    {
        public static T ToEnum<T>(this string s) => (T)Enum.Parse(typeof(T), s);

        public static IEnumerable<T> ToEnum<T>(this IEnumerable<string> arr)
            => arr.Select(s => (T)Enum.Parse(typeof(T), s));
    }
}
