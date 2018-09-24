using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.Threading;

namespace AsmodatStandard.Types
{
    public class KVLocker
    {
        /// <summary>
        /// Note: ConcurrentDictionary Is Not Always Thread-Safe
        /// </summary>
        private ConcurrentDictionary<string, SemaphoreSlim> _dictionary;

        private readonly SemaphoreSlim _locker;

        public KVLocker()
        {
            _locker = new SemaphoreSlim(1, 1);
            _dictionary = new ConcurrentDictionary<string, SemaphoreSlim>();
        }

        public SemaphoreSlim GetLock(string key)
            => GetLocks(key).Single();

        public SemaphoreSlim[] GetLocks(params string[] keys)
            => GetLocks(keys.ToIEnumerable());

        public SemaphoreSlim[] GetLocks(IEnumerable<string> keys)
        {
            var keysArr = keys?.ToArray();

            if (keys == null || keysArr.Any(k => k.IsNullOrEmpty()))
                throw new ArgumentException("No key can be null or empty and at least one key must be present.");

            if (keysArr.Length != keys.Distinct().Count())
                throw new ArgumentException("All keys must be distinct.");

            var results = new List<SemaphoreSlim>();

            _locker.Lock(() =>
             {
                 var keysToDispose = _dictionary.Where(x => !keys.Contains(x.Key) && !x.Value.IsLocked()).Select(x => x.Key);

                 if (!keysToDispose.IsNullOrEmpty())
                     _dictionary.TryRemove(keysToDispose, out var values);

                 foreach (var key in keysArr)
                 {
                     if (!_dictionary.ContainsKey(key))
                         _dictionary.Add(key, new SemaphoreSlim(1, 1));

                     results.Add(_dictionary[key]);
                 }
             });

            return results.ToArray();
        }
    }
}
