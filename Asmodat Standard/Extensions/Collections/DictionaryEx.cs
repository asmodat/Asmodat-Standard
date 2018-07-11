using System.Collections.Generic;

namespace AsmodatStandard.Extensions.Collections
{
    public static class DictionaryEx
    {
        public static V GetValueOrDefault<K, V>(this Dictionary<K, V> source, K key, V @default = default(V))
            => source.TryGetValue(key, out var result) ? result : @default;
    }
}
