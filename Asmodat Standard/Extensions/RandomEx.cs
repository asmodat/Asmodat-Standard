﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class RandomEx
    {
        public static readonly Random Instance = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Random value from the range of [min, max)
        /// </summary>
        public static int Next(int min, int max) => Instance.Next(min, max);

        /// <summary>
        /// Random value from the range of [0, max)
        /// </summary>
        public static int Next(int max) => Instance.Next(max);

        /// <summary>
        /// Random int arrary of the length 'count' with values from the range of [min, max)
        /// </summary>
        public static int[] Next(int min, int max, int count)
        {
            var arr = new int[count];
            for (int i = 0; i < count; i++)
                arr[i] = Next(min, max);
            return arr;
        }

        /// <summary>
        /// Fisher-Yeates shuffle: https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = Next(n--);
                var tmp = array[n];
                array[n] = array[k];
                array[k] = tmp;
            }
        }

        public static T[] Shuffle<T>(this IEnumerable<T> items)
        {
            var arr = items.ToArray();
            arr.Shuffle();
            return arr;
        }

        /// <summary>
        /// Random distinct int arrary of the length 'count' with values from the range of [min, max)
        /// </summary>
        public static int[] NextDistinct(int min, int max, int count)
        {
            var maxDistinctElements = (max - min);
            if (maxDistinctElements < count)
                throw new ArgumentException("Number of distinct elements is less then possible count.");

            var arr = Enumerable.Range(min, maxDistinctElements).ToList();
            var result = new int[count];
            int i = -1;
            while(++i < count)
            {
                var idx = Next(0, arr.Count);
                result[i] = arr[idx];
                arr.RemoveAt(idx);
            }
            return result;
        }
        
        public static T[] SelectRandomDistinct<T>(this IEnumerable<T> items, int? count = null)
        {
            var arr = items.ToArray();
            var length = Math.Min(arr.Length, count ?? arr.Length);
            var indexes = NextDistinct(0, arr.Length, length);
            var result = new T[length];
            for (int i = 0; i < length; i++)
                result[i] = arr[indexes[i]];

            return result;
        }
    }
}
