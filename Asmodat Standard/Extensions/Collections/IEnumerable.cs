using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Collections
{
    public static class IEnumerableEx
    {
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

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> left, params T[] right) => left?.ToArray().Merge(right);

        public static IEnumerable<T> Merge<T>(this IEnumerable<T> left, IEnumerable<T> right) => left?.ToArray().Merge(right?.ToArray());

        public static string JoinToString(this IEnumerable<char> coll) => coll == null ? null : new string(coll.ToArray());

        /// <summary>
        /// returns common elements for two sets of enumerables
        /// </summary>
        public static IEnumerable<T> Intersection<T>(this IEnumerable<T> e1, IEnumerable<T> e2)
            => e1.Intersection(new HashSet<T>(e2));

        public static IEnumerable<T> Intersection<T>(this IEnumerable<T> e1, HashSet<T> lookup)
        {
            if (e1 == null || lookup == null)
                return null;

            return e1.Where(lookup.Contains);
        }

        public static int TryCount<T>(this IEnumerable<T> enumerable, int _default = 0)
        {
            if (enumerable == null)
                return _default;

            try
            {
                return enumerable.Count();
            }
            catch
            {
                return _default;
            }
        }

        public static long TryLongCount<T>(this IEnumerable<T> enumerable, long _default = 0)
        {
            if (enumerable == null)
                return _default;

            try
            {
                return enumerable.LongCount();
            }
            catch
            {
                return _default;
            }
        }

        public static IEnumerable<T> Clone<T>(this IEnumerable<T> enumerable) where T : ICloneable
        {
            if (enumerable == null) return null;
            else return enumerable.Select(o => (T)o.Clone());
        }


        public static TSource[] DistinctArray<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null) return null;
            else return source.Distinct().ToArray();
        }

        /// <summary>
        /// Checks if Enumerable is null or it's count is less or equal zero.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source == null || source.Count() <= 0)
                return true;
            else return false;
        }

        /// <summary>
        /// Checks if enumerable count is less then value
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsCountLessThen<TSource>(this IEnumerable<TSource> source, int value)
        {
            if (source == null || source.Count() < value)
                return true;
            else return false;
        }

        public static bool IsCountGreaterThen<TSource>(this IEnumerable<TSource> source, int value)
        {
            int? cnt = source?.Count();
            return cnt != null && cnt.Value > value;
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
            if (source == null || source.Count() <= value)
                return true;
            else return false;
        }

        public static bool IsCountGreaterOrEqual<TSource>(this IEnumerable<TSource> source, int value)
        {
            int? cnt = source?.Count();
            return cnt != null && cnt.Value >= value;
        }

        public static int GetCount<TSource>(this IEnumerable<TSource> source)
        {
            //IEnumerableEx.IsNullOrEmpty(source);

            if (source == null || source.LongCount() <= 0)
                return 0;
            else return source.Count();
        }

        public static long GetLongCount<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null || source.LongCount() <= 0)
                return 0;
            else return source.LongCount();
        }

        public static bool IsCountEqual<TSource>(this IEnumerable<TSource> source, int value)
        {
            if (source != null && source.Count() == value)
                return true;
            else return false;
        }

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
    }
}