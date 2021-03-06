﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Json;

namespace AsmodatStandard.Extensions.Collections
{
    public static class DictionaryEx
    {
        public static V GetValueOrDefault<K, V>(this Dictionary<K, V> source, K key, V @default = default(V))
            => source.TryGetValue(key, out var result) ? result : @default;

        public static V GetFirstValueOrDefault<K, V>(this Dictionary<K, V> source, params K[] keys)
        {
            foreach(var k in keys)
                if (source.TryGetValue(k, out var result))
                    return result;

            return default(V);
        }

        public static V GetOrThrow<K, V>(this Dictionary<K, V> source, K key)
        {
            try
            {
                return source[key];
            }
            catch(Exception ex)
            {
                throw new Exception($"Key '{key?.JsonSerialize()??"null"}' was not found", ex);
            }
        }

        public static bool CollectionEquals(
            this IDictionary<string, string> c1,
            IDictionary<string, string> c2,
            bool trim = false,
            bool ignoreCase = false)
        {
            if (c1 == null && c2 == null)
                return true;

            if (c1 == null || c2 == null)
                return false;

            if (c1.Count != c2.Count)
                return false;


            foreach (var k in c1.Keys)
            {
                if (!c2.ContainsKey(k))
                    return false;

                var v1 = c1[k];
                var v2 = c2[k];

                if (v1 == null && v2 == null)
                    continue;

                if (v1 == null || v2 == null)
                    continue;

                if (trim)
                {
                    v1 = v1?.Trim();
                    v2 = v2?.Trim();
                }

                if (ignoreCase)
                {
                    v1 = v1?.ToLower();
                    v2 = v2?.ToLower();
                }

                if (v1 != v2)
                    return false;
            }

            return true;
        }
    }
}
