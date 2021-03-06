﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace AsmodatStandard.Extensions.Collections
{
    public static class ArrayEx
    {
        /// <summary>
        /// [width,height]
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Width<TKey>(this TKey[,] source)
        {
            if (source == null)
                return 0;

            return source.GetLength(0);
        }
        /// <summary>
        /// [width,height]
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Height<TKey>(this TKey[,] source)
        {
            if (source == null)
                return 0;

            return source.GetLength(1);
        }


        public static bool IsAnyDimentionNullOrEmpty<TKey>(this TKey[,,] source)
        {
            if (source == null)
                return true;

            int i = 0, r = source.Rank;
            for (; i < r; i++)
                if (source.GetLength(i) <= 0)
                    return true;

            return false;
        }

        /// <summary>
        /// [width,height,depp]
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Width<TKey>(this TKey[,,] source)
        {
            if (source == null)
                return 0;

            return source.GetLength(0);
        }
        /// <summary>
        /// [width,height,depp]
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Height<TKey>(this TKey[,,] source)
        {
            if (source == null)
                return 0;

            return source.GetLength(1);
        }

        /// <summary>
        /// [width,height,depth]
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Depth<TKey>(this TKey[,,] source)
        {
            if (source == null)
                return 0;

            return source.GetLength(2);
        }

        public static T TryGetValueOrDefault<T>(this T[] arr, int index, T @default = default(T))
            => (arr.IsNullOrEmpty() || arr.Length <= index) ? @default : arr[index];

        public static T[] Reverse<T>(this T[] arr)
        {
            var newArr = new T[arr.Length];
            for (int i = arr.Length - 1, i2 = 0; i >= 0; i--, i2++)
                newArr[i2] = arr[i];

            return newArr;
        }

        /// <summary>
        /// Takes last 'n' values starting at position 'position' excluding 'position' index itself
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <param name="n"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T[] TakeLastWithRotation<T>(this T[] arr, int n, int position)
        {
            if (n > arr.Length)
                throw new ArgumentException("TakeLastWithRotation, parameter n can't be greater then arr length");

            if (n == 0)
                return new T[0];

            var shift = Math.Max(position - n, 0);
            var fromBeggining = arr.SubArray(shift, position - shift);

            if (n <= position)
                return fromBeggining;

            var fromEnd = arr.SubArray(arr.Length - (n - position), n - position);

            return fromEnd.Merge(fromBeggining);
        }


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
