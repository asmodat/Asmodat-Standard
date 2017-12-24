using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AsmodatStandard.Extensions
{
    public static class StringEx
    {
        public static string CoalesceNullOrEmpty(this string str, string value) => str.IsNullOrEmpty() ? value : str;
        public static string CoalesceNullOrWhitespace(this string str, string value) => str.IsNullOrWhitespace() ? value : str;

        public static string ReplaceNonASCII(this string str, string replaceWith) => Regex.Replace(str, @"[^\u0000-\u007F]+", replaceWith);

        public static string SkipChars(this string str, int count) => str.Substring(count, str.Length - count);

        public static bool StartsWithAny(this string str, params string[] any) => any?.Any(s => str.StartsWith(s)) == true;

        public static bool EndsWithAny(this string str, params string[] any) => any?.Any(s => str.EndsWith(s)) == true;

        public static bool IsEmpty(this string str) => str == string.Empty;

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhitespace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool Equals(this string left, string right, StringComparison stringComparison) => string.Equals(left, right, stringComparison);
    }
}
