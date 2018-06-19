﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Collections
{
    public static class IEnumerableEx
    {
        public static IEnumerable<T> ToIEnumerable<T>(this IEnumerable<T> arr) => arr.Cast<T>();

        public static IEnumerable<T> ConcatOrDefault<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            if (left == null && right == null)
                return null;

            if (left == null)
                return right;

            if (right == null)
                return left;

            var list = new List<T>();
            list.AddRange(right);
            list.AddRange(left);
            return list;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> collections)
        {
            if (collections == null)
                return null;

            var result = new List<T>();
            foreach (var collection in collections)
                if(collection != null)
                    result.AddRange(collection);

            return result;
        }

        public static double AverageOrDefault(this IEnumerable<double> source, double @default = double.NaN)
            => source.IsNullOrEmpty() ? @default : source.Average();

        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source)
            => source?.SelectMany(x => x);
        
            /// <summary>
            /// splits source into (up to) n elemets
            /// </summary>
        public static List<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int n)
        {
            var length = source.Count();
            var countPerSplit = (int)Math.Ceiling((double)length / n);

            var result = new List<IEnumerable<T>>();
            int toSkip;
            for(int i = 0; i < n; i++)
            {
                toSkip = i * countPerSplit;
                if (toSkip >= length)
                    break;

                result.Add(source.Skip(toSkip).Take(countPerSplit));
            }

            return result;
        }

        public static async Task<IEnumerable<K>> SelectManyAsync<T, K>(this IEnumerable<T> enumeration, Func<T, Task<IEnumerable<K>>> func)
            => (await Task.WhenAll(enumeration.Select(func))).SelectMany(s => s);

        public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector)
        {
            var hashSet = new HashSet<K>();
            foreach (T item in source)
                if (hashSet.Add(keySelector(item)))
                    yield return item;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);

        public static T SelectMin<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) 
            => source.SortAscending(keySelector).FirstOrDefault();

        public static T SelectMax<T, K>(this IEnumerable<T> source, Func<T, K> keySelector)
            => source.SortDescending(keySelector).FirstOrDefault();

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> left, params T[] right) 
            => (left?.ToArray()).Merge(right);

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> left, IEnumerable<T> right) 
            => (left?.ToArray()).Merge(right?.ToArray());

        public static string JoinToString(this IEnumerable<char> coll) 
            => coll == null ? null : new string(coll.ToArray());

        /// <summary>
        /// returns common elements for two sets of enumerables
        /// </summary>
        public static IEnumerable<T> Intersection<T>(this IEnumerable<T> e1, IEnumerable<T> e2)
            => e1.Intersection(new HashSet<T>(e2));

        public static IEnumerable<T> Intersection<T>(this IEnumerable<T> e1, HashSet<T> lookup)
            => (e1 == null || lookup == null) ? ( e1 == null && lookup == null ? null : new T[0]) : e1.Where(lookup.Contains);

        public static IEnumerable<T> Clone<T>(this IEnumerable<T> enumerable) where T : ICloneable
        {
            if (enumerable == null) return null;
            else return enumerable.Select(o => (T)o.Clone());
        }
        
        public static TSource[] DistinctArray<TSource>(this IEnumerable<TSource> source)
            => source?.Distinct().ToArray();

        /// <summary>
        /// Checks if Enumerable is null or it's count is less or equal zero.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
            => source == null || !source.Any();

        /// <summary>
        /// Checks if enumerable count is less then value
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsCountLessThen<TSource>(this IEnumerable<TSource> source, int value)
        {
            var cntr = -1;
            foreach (var v in source)
                if (++cntr > value)
                    return false;
            return true;
        }

        public static bool IsCountGreaterThen<TSource>(this IEnumerable<TSource> source, int value)
        {
            var cntr = -1;
            foreach (var v in source)
                if (++cntr > value)
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if enumerable count is less or equal value
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsCountLessOrEqual<TSource>(this IEnumerable<TSource> source, int value)
        {
            var cntr = -1;
            foreach (var v in source)
                if (++cntr >= value)
                    return false;
            return true;
        }

        public static bool IsCountGreaterOrEqual<TSource>(this IEnumerable<TSource> source, int value)
        {
            int cntr = -1;
            foreach (var v in source)
                if (++cntr >= value)
                    return true;
            return false;
        }

        public static int CountOrDefault<TSource>(this IEnumerable<TSource> source)
            => source.IsNullOrEmpty() ? 0 : source.Count();

        public static long LongCountOrDefault<TSource>(this IEnumerable<TSource> source)
            => source.IsNullOrEmpty() ? 0 : source.LongCount();

        public static bool IsCountEqual<TSource>(this IEnumerable<TSource> source, int value)
            => source.Count() == value;

        /// <summary>
        /// Compares count of collections, 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="collection1"></param>
        /// <param name="collection2"></param>
        /// <returns>if any of collections is undefined (null) or count is unequal then false is returned</returns>
        public static bool EqualsCount<TSource>(this IEnumerable<TSource> collection1, IEnumerable<TSource> collection2)
        {
            if (collection1 != null && collection2 != null && collection1.Count() == collection2.Count())
                return true;
            else return false;
        }

        public static IEnumerable<TSource> SortAscending<TSource, Tkey>(this IEnumerable<TSource> source, Func<TSource, Tkey> keySelector)
        {
            if (source == null || keySelector == null || source.Count() <= 1)
                return source;

            return source.OrderBy(keySelector);
        }

        public static IEnumerable<TSource> SortDescending<TSource, Tkey>(this IEnumerable<TSource> source, Func<TSource, Tkey> keySelector)
        {
            if (source == null || source.Count() <= 1)
                return source;

            if (keySelector == null)
                return null;

            return source.OrderByDescending(keySelector);
        }
        
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
        {
            if (source == null || source.Count() < n)
                return null;

            return source.Skip(Math.Max(0, source.Count() - n));
        }

        /// <summary>
        /// Executes action with index and returns count, use action as: (v,i) => {}, where v is valie, i is index
        /// </summary>
        public static int ForEach<T>(this IEnumerable<T> source, Action<T, int> a)
        {
            int i = -1;
            foreach (T item in source)
                a(item, ++i);

            return ++i;
        }

        /// <summary>
        /// Executes action and returns count
        /// </summary>
        public static int ForEach<T>(this IEnumerable<T> source, Action<T> a)
        {
            int i = -1;
            foreach (T item in source)
            {
                a(item);
                ++i;
            }

            return ++i;
        }

        public static int ParallelForEach<T>(this IEnumerable<T> source, Action<T> a)
        {
            int i = -1;
            Parallel.ForEach(source, item =>
            {
                a(item);
                ++i;
            });

            return ++i;
        }

        public static async Task<int> ForEach<T>(this IEnumerable<T> source, Func<T, Task> a)
        {
            int i = -1;
            foreach (T item in source)
            {
                await a(item);
                ++i;
            }

            return ++i;
        }

        public static async Task ParallelForEach<T>(this IEnumerable<T> source, Func<T, Task> a)
        {
            var tasks = new List<Task>();
            foreach (T item in source)
                tasks.Add(a(item));

            await Task.WhenAll(tasks);
        }
    }
}