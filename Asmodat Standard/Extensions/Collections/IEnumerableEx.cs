using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Collections
{
    public static class IEnumerableEx
    {
        public static T GetRandomOrDefault<T>(this IEnumerable<T> source, T @default = default(T)) where T : IComparable
        {
            var arr = source.ToArray();
            if (arr.IsNullOrEmpty())
                return @default;

            return arr[RandomEx.Next(0, arr.Length)];
        }

        public static T MaxOrDefault<O,T>(this IEnumerable<O> source, Func<O, T> selector, T @default = default(T)) where T : IComparable
        {
            if (source.IsNullOrEmpty())
                return @default;

            return source.Max(selector);
        }

        public static T MinOrDefault<O, T>(this IEnumerable<O> source, Func<O, T> selector, T @default = default(T)) where T : IComparable
        {
            if (source.IsNullOrEmpty())
                return @default;

            return source.Min(selector);
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> source, T @default = default(T)) where T : IComparable
        {
            if (source.IsNullOrEmpty())
                return @default;

            return source.Max();
        }

        public static T MinOrDefault<T>(this IEnumerable<T> source, T @default = default(T)) where T : IComparable
        {
            if (source.IsNullOrEmpty())
                return @default;

            return source.Min();
        }

        public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<KeyValuePair<K, V>> source)
        {
            if (source == null)
                return null;

            if (source.IsNullOrEmpty())
                return new Dictionary<K, V>();

            return source.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, T @default = default(T))
         => source.ElementAtOrDefault(0, @default);

        public static T SecondOrDefault<T>(this IEnumerable<T> source, T @default = default(T))
         => source.ElementAtOrDefault(1, @default);

        public static T ThirdOrDefault<T>(this IEnumerable<T> source, T @default = default(T))
         => source.ElementAtOrDefault(2, @default);

        public static T ElementAtOrDefault<T>(this IEnumerable<T> source, int index, T @default = default(T))
        {
            var arr = source?.ToArray();
            if (arr.IsNullOrEmpty() || arr.Length <= index)
                return @default;

            return arr[index];
        }

        ///<summary>Finds the index of the first item matching an expression in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="predicate">The expression to test the items against.</param>
        ///<returns>The index of the first matching item, or -1 if no items match.</returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (predicate == null) throw new ArgumentNullException("predicate");

            int retVal = 0;
            foreach (var item in items)
            {
                if (predicate(item)) return retVal;
                retVal++;
            }
            return -1;
        }

        ///<summary>Finds the index of the first occurrence of an item in an enumerable.</summary>
        ///<param name="items">The enumerable to search.</param>
        ///<param name="item">The item to find.</param>
        ///<returns>The index of the first matching item, or -1 if the item was not found.</returns>
        public static int IndexOf<T>(this IEnumerable<T> items, T item) { return items.FindIndex(i => EqualityComparer<T>.Default.Equals(item, i)); }

        public static bool IntersectAny<T>(this IEnumerable<T> e1, IEnumerable<T> e2)
        {
            if (e1.IsNullOrEmpty() || e2.IsNullOrEmpty())
                return false;

            return e1.Intersect(e2).Any();
        }

        public static T[] ToEvenArray<T>(this IEnumerable<T> values)
        {
            var arr = values.ToArray();

            if (arr.Length <= 1)
                return new T[0];

            if (arr.Length % 2 == 0)
                return arr;

           return arr.SubArray(0, arr.Length - 1);
        }

        public static T[] ToOddArray<T>(this IEnumerable<T> values)
        {
            var arr = values.ToArray();

            if (arr.Length <= 0)
                return null;

            if (arr.Length % 2 != 0)
                return arr;

            return arr.SubArray(0, arr.Length - 1);
        }

        public static string StringJoin<T>(this IEnumerable<T> values, string separator = ",")
            => string.Join<T>(separator, values);

        public static string StringPairJoin<T>(this IEnumerable<T> values, string pairSeparator = "-", string separator = ",")
        {
            var arr = values.ToArray();
            if (arr.Length % 2 != 0)
                throw new ArgumentException($"Can't pair join collection with odd number of items ({arr.Length})");

            if (arr.Length == 0)
                return string.Empty;

            var sb = new StringBuilder();
            for(int i = 0; i < arr.Length; i += 2)
                sb.Append($"{arr[i]}{pairSeparator}{arr[i + 1]}{separator}");
            
            return sb.ToString().Substring(0, sb.Length - separator.Length);
        }

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

        /// <summary>
        /// Slices collection into groups of max n elements each
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int count)
        {
            if (count <= 0)
                throw new ArgumentException($"count can't be less or equal 0, but was {count}");

            var list = new List<T>();
            foreach(var e in source)
            {
                list.Add(e);

                if (list.Count == count)
                {
                    yield return list;
                    list = new List<T>();
                }
            }

            if (!list.IsNullOrEmpty())
                yield return list;
        }

        public static async Task<IEnumerable<K>> SelectManyAsync<T, K>(this IEnumerable<T> enumeration, Func<T, Task<IEnumerable<K>>> func)
            => (await Task.WhenAll(enumeration.Select(func))).SelectMany(s => s);

        public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector)
            => source.GroupBy(keySelector).Select(x => x.First());

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

        public static int CountOrDefault<TSource>(this IEnumerable<TSource> source, int @default = 0)
            => source.IsNullOrEmpty() ? @default : source.Count();

        public static long LongCountOrDefault<TSource>(this IEnumerable<TSource> source, long @default = 0)
            => source.IsNullOrEmpty() ? @default : source.LongCount();

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

        public static IEnumerable<T> TakeLastN<T>(this IEnumerable<T> source, int n) => TakeLast(source, n);

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
            // Validate
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (a == null)
            {
                throw new ArgumentNullException("action");
            }

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

        /// <summary>
        /// foreach that passess result of all previous operations back into the function
        /// </summary>
        public static IEnumerable<T2> ForEachWithAllPrevious<T1, T2>(this IEnumerable<T1> source, Func<T1, IEnumerable<T2>, T2> a)
        {
            var items = new List<T2>();
            foreach (T1 item in source)
                items.Add(a(item, items));

            return items;
        }

        /// <summary>
        /// ForEach that passess result of a previous operations back into the function and returns the last result
        /// </summary>
        public static T2 ForEachWithPrevious<T, T2>(this IEnumerable<T> source, Func<T, T2, T2> a, T2 initial = default(T2))
        {
            var previous = initial;
            foreach (T item in source)
                previous = a(item, previous);

            return previous;
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