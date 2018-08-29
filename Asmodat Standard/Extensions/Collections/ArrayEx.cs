using System;
using System.Linq;
using System.Text;

namespace AsmodatStandard.Extensions.Collections
{
    public static class ArrayEx
    {
        public static string ToString(this byte[] source, Encoding encoding)
            => encoding.GetString(source);

        public static void CopyTo<T>(this T[] source, T[] destination, int destinationIndex)
            => Array.Copy(source, 0, destination, destinationIndex, source.Length);

        public static void CopyTo<T>(this T[] source, int sourceIndex, T[] destination, int destinationIndex)
            => Array.Copy(source, sourceIndex, destination, destinationIndex, source.Length - sourceIndex);

        public static void CopyTo<T>(this T[] source, int sourceIndex, T[] destination, int destinationIndex, int length)
            => Array.Copy(source, sourceIndex, destination, destinationIndex, length);

        public static T GetValueOrDefault<T>(this T[][] arr, int x, int y, T @default = default(T))
            => (arr.IsNullOrEmpty() ||  arr.Height() <= y || arr[y].Length <= x) ? @default : arr[y][x];
        
        public static int MaxWidth<T>(this T[][] arr) => arr?.Max(x => x.Length) ?? 0;
        public static int MinWidth<T>(this T[][] arr) => arr?.Min(x => x.Length) ?? 0;
        public static int Height<T>(this T[][] arr) => arr?.Length ?? 0;

        /// <summary>
        /// resizes input array into provided dimentions, if dimention is null original value will be set
        /// </summary>
        public static T[][] ResizeRectangular<T>(this T[][] arr, int? height = null, int? width = null, 
            bool heightNotLessThenOriginal = false, bool widthNotLessThenOriginal = false)
        {
            height = height ?? arr.Height();
            width = width ?? arr.MaxWidth();

            if (heightNotLessThenOriginal)
                height = Math.Max(height.Value, arr.Height());

            if (widthNotLessThenOriginal)
                width = Math.Max(width.Value, arr.MaxWidth());

            if (height < 0 || width < 0)
                throw new ArgumentException("height nor width cannot be negative");

            var output = new T[height.Value][];

            for (int y = 0; y < output.Length; y++)
            {
                output[y] = new T[width.Value];

                if (y < arr.Height())
                    for (int x = 0; x < width && x < (arr[y]?.Length ?? 0); x++)
                        output[y][x] = arr[y][x];
            }

            return output;
        }

        public static T[] Merge<T>(this T[] left, params T[][] right)
            => left.Merge(right.Merge());

        public static T[] Merge<T>(this T[][] arrays)
        {
            if (arrays == null)
                throw new ArgumentNullException(nameof(arrays));

            if (arrays.Length == 0)
                return new T[0];

            var result = new T[arrays.Select(x => x?.Length ?? 0).Sum()];
            var position = 0;
            for(int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i] == null)
                    continue;

                Array.Copy(arrays[i], 0, result, position, arrays[i].Length);
                position += arrays[i].Length;
            }

            if (position == 0 && arrays.All(x => x == null))
                return null; //return null if all are null

            return result;
        }

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
