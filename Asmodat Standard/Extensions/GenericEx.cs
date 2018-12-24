using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using AsmodatStandard.Extensions.Types;

namespace AsmodatStandard.Extensions
{
    public static class GenericEx
    {
        public static T ConcatNull<T>(this T obj1, T obj2) => obj1 == null ? obj2 : obj1;

        public static bool IsDefault<T>(this T value) where T : struct
            => value.Equals(default(T));

        public static bool EquailsAny<T>(this T o, params T[] others) where T : IEquatable<T>
            => o.EquailsAny<T, T>(others);

        /// <summary>
        /// checks if all elements of sequence a2 are contained in a1
        /// </summary>
        public static bool ContainsAll<T>(this T[] a1, params T[] a2) where T : IEquatable<T>
        {
            var dA1 = a1.Distinct();
            var dA2 = a2.Distinct();

            if (dA2.Length != dA2.Length)
                return false;

            foreach (var v in dA1)
                if (!v.EquailsAny<T>(dA2))
                    return false;

            return true;
        }

        /// <summary>
        /// checks if all elements of sequence a2 are contained in a1
        /// </summary>
        public static bool ContainsAll<T1, T2>(this T1[] a1, params T2[] a2) where T1 : IEquatable<T2>
        {
            var dA1 = a1.Distinct();
            var dA2 = a2.Distinct();

            if (dA2.Length != dA2.Length)
                return false;

            foreach (var v in dA1)
                if (!v.EquailsAny(dA2))
                    return false;

            return true;
        }

        public static bool EquailsAny<T1, T2>(this T1 o, params T2[] others) where T1 : IEquatable<T2>
        {
            if (others.IsNullOrEmpty())
                return false;

            foreach (var other in others)
                if ((other == null && o == null) || other.Equals(o))
                    return true;

            return false;
        }

        public static string JsonSerializeAsPrettyException<T>(this T ex, 
            Formatting formatting = Formatting.Indented, 
            int maxDepth = 1000,
            int stackTraceMaxDepth = 1000
            ) where T : Exception
        {
            
            if (ex == null)
                return null;

            return ex.ToPreetyException(maxDepth: maxDepth, stackTraceMaxDepth: stackTraceMaxDepth).ToString(formatting);
        }

        public static T Max<T>(params T[] parameters) where T : IEnumerable<T> => parameters.Max();
        public static T Min<T>(params T[] parameters) where T : IEnumerable<T> => parameters.Min();

        public static string StringJoin<T>(this T[] arr, string separator) => string.Join(separator, arr);
    }
}
