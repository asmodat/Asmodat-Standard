using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsmodatStandard.Extensions
{
    public static class RandomEx
    {
        public static readonly Random Instance = new Random(Guid.NewGuid().GetHashCode());

        public static byte[] NextBytes(int length)
        {
            var arr = new byte[length];
            Instance.NextBytes(arr);
            return arr;
        }

        public static string NextHexString(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            Instance.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + Instance.Next(16).ToString("X");
        }

        public static string NextString(int length, int minCharCode, int maxCharCode)
        {
            var max = maxCharCode + 1;
            var builder = new StringBuilder(length);
            var random = new Random();
            for (var i = 0; i < length; i++)
                builder.Append((char)Next(minCharCode, max));
            
            return builder.ToString();
        }

        public static string NextAlphanumeric(int length)
        {
            var builder = new StringBuilder(length);
            var random = new Random();
            int iChar;
            for (var i = 0; i < length; i++)
            {
                do
                {
                    iChar = Next(48, 123);
                } while (
                !((iChar >= 48 && iChar <= 57) || //number
                (iChar >= 65 && iChar <= 90) || //uppercase
                (iChar >= 97 && iChar <= 122))); //lowercase

                builder.Append((char)iChar);
            }

            return builder.ToString();
        }

        public static string NextStringASCI(int length)
            => NextString(length, 0, 127);

        public static double NextDouble(double min, double max)
        {
            if (min > max)
                throw new ArgumentException($"{nameof(min)} can't be gteater then {nameof(max)}, but was min: {min}, max: {max}");

            return Instance.NextDouble() * (max - min) + min;
        }

        public static DateTime DateTime(DateTimeKind kind = DateTimeKind.Utc)
            => new DateTime(Next(0, (TimeSpan.TicksPerDay*(long)(365.25 * 9996))), kind);

        public static bool NextBool()
            => Instance.Next() % 2 == 0 ? true : false;

        public static long Next(long min, long max)
        {
            if (max <= min)
                throw new ArgumentOutOfRangeException($"max must be > min, Min: {min}, Max: {max}");

            ulong uRange = (ulong)(max - min);

            //Modolo bias; see https://stackoverflow.com/a/10984975/238419
            ulong ulongRand;
            do
            {
                var buf = new byte[8];
                Instance.NextBytes(buf);
                ulongRand = (ulong)BitConverter.ToInt64(buf, 0);
            } while (ulongRand > ulong.MaxValue - ((ulong.MaxValue % uRange) + 1) % uRange);

            return (long)(ulongRand % uRange) + min;
        }

        /// <summary>
        /// Random value from the range of [min, max)
        /// </summary>
        public static int Next(int min, int max) => Instance.Next(min, max);

        public static int NextEven(int min, int max)
        {
            if (min == (max-1) && min % 2 != 0)
                throw new ArgumentException($"Not a single even value is present within range <{min};{max})");

            var val = Instance.Next(min, max);

            if (val % 2 == 0)
                return val;

            return (val + 1 >= max) ? --val : ++val;
        }

        public static int NextOdd(int min, int max)
        {
            if (min == (max - 1) && min % 2 == 0)
                throw new ArgumentException($"Not a single odd value is present within range <{min};{max})");

            var val = Instance.Next(min, max);

            if (val % 2 != 0)
                return val;

            return (val + 1 >= max) ? --val : ++val;
        }

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
