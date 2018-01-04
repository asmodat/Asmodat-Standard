using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class LinqEx
    {
        public static IEnumerable<K> SelectMany<K>(params IEnumerable<K>[] elements)
            => elements.SelectMany(x => x);
    }
}
