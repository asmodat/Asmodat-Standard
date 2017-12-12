using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace AsmodatStandard.Extensions.Collections
{
    public static class ArrayEx
    {
        /// <summary>
        /// joins two arrays together
        /// </summary>
        public static T[] Merge<T>(this T[] left, params T[] right)
        {
            if (left == null && right == null)
                return null;

            var arr = new T[(left?.Length ?? 0) + (right?.Length ?? 0)];

            if (left != null)
                Array.Copy(left, arr, left.Length);

            if (right != null)
                Array.Copy(right, 0, arr, (left?.Length ?? 0), right.Length);

            return arr;
        }

        /// <summary>
        /// Ugly and slow array distinct, returns array of unique/distinc values using default equity comparer
        /// </summary>
        public static T[] Distinct<T>(this T[] arr) => arr?.ToList()?.Distinct()?.ToArray();

        public static string JoinToString(this char[] arr) => arr == null ? null : new string(arr);

        /// <summary>
        /// returns sub array starting at index and ending at length or end of the array => (data.length - index)
        /// </summary>
        public static T[] SubArray<T>(this T[] data, int index, int length = int.MaxValue)
        {
            if (index < 0 || index >= data.Length)
                throw new ArgumentException("index canno't be less then 0 nor greater or equal data array length");

            if (length < 0)
                throw new ArgumentException("length cannot be negative");
            else if (length == 0)
                return new T[0];

            var result = new T[Math.Min(data.Length - index, length)];
            Array.Copy(data, index, result, 0, result.Length);
            return result;
        }
    }
}
