using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AsmodatStandard.Extensions.Collections
{
    public static class ConcurrentBagEx
    {
        public static void AddRange<T>(this ConcurrentBag<T> bag, IEnumerable<T> range)
        {
            if (range.IsNullOrEmpty())
                return;

            foreach(var v in range)
                bag.Add(v);
        }

        public static void AddRange<T>(this ConcurrentBag<T> bag, params T[] range)
        {
            if (range.IsNullOrEmpty())
                return;

            foreach (var v in range)
                bag.Add(v);
        }
    }
}
