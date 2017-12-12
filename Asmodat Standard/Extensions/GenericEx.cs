using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class GenericEx
    {
        public static T Max<T>(params T[] parameters) where T : IEnumerable<T> => parameters.Max();
        public static T Min<T>(params T[] parameters) where T : IEnumerable<T> => parameters.Min();

        public static string StringJoin<T>(this T[] arr, string separator) => string.Join(separator, arr);
    }
}
