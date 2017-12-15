using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Collections
{
    public static class IEnumerableEx
    {
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
        {
            if (e1 == null || e2 == null)
                return null;

            var lookup = new HashSet<T>(e2);
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



        /// <summary>
        /// Removes repeating values inside table
        /// Use Example:
        /// IEnumerable[Foo] distinct = someList.DistinctBy(x => x.FooProperty);
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, Tkey>(this IEnumerable<TSource> source, Func<TSource, Tkey> keySelector)
        {
            if (source == null || keySelector == null || source.Count() <= 1)
                return source;

            var knownKeys = new HashSet<Tkey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
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

        /// <summary>
        /// Distinctincts list by key property, and then adds to dictionary
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <param name="valueSelector"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TElement> ToDistinctKeyDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> valueSelector)
        {
            if (source == null || keySelector == null || valueSelector == null)
                return null;

            if (source.Count() <= 0)
                return new Dictionary<TKey, TElement>();

            List<TSource> distinct = source.DistinctBy(keySelector).ToList();

            if (distinct == null)
                return null;

            if (distinct.Count <= 0)
                return new Dictionary<TKey, TElement>();

            return distinct.ToDictionary(keySelector, valueSelector);
        }
        
        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
        {
            if (source == null || source.Count() < n)
                return null;

            return source.Skip(Math.Max(0, source.Count() - n));
        }

        /// <summary>
        /// Executes action with index and returns count
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