﻿using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class StringEx
    {
        public static bool StartsWithAny(this string str, params string[] any) => any?.Any(s => str.StartsWith(s)) == true;

        public static bool EndsWithAny(this string str, params string[] any) => any?.Any(s => str.EndsWith(s)) == true;

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhitespace(this string str) => str.IsNullOrWhitespace();
    }
}
