using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsmodatStandard.Extensions.Collections
{
    public static class ListEx
    {
        public static void AddIfNotNull<T>(this List<T> list, params T[] items)
        {
            var notNullItems = items.Where(x => x != null);

            if (!notNullItems.IsNullOrEmpty())
                list.AddRange(notNullItems);
        }

        public static void AddMany<T>(this List<T> list, params T[] items)
            => list.AddRange(items);
    }
}