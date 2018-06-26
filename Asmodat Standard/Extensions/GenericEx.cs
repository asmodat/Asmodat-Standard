using AsmodatStandard.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class GenericEx
    {
        public static string JsonSerializeAsPrettyException<T>(this T ex, Formatting formatting = Formatting.Indented, int maxDepth = 1000) where T : Exception
        {
            if (ex == null)
                return null;

            return (new PreetyException(ex, maxDepth)).JsonSerialize(formatting);
        }

        public static T Max<T>(params T[] parameters) where T : IEnumerable<T> => parameters.Max();
        public static T Min<T>(params T[] parameters) where T : IEnumerable<T> => parameters.Min();

        public static string StringJoin<T>(this T[] arr, string separator) => string.Join(separator, arr);
    }
}
