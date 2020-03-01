using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Threading;
using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace AsmodatStandard.Types
{
    public class SemaphoresNamed
    {
        private SemaphoreSlim locker = new SemaphoreSlim(1, 1);
        private ConcurrentDictionary<string, SemaphoreSlim> sempahores;
        private string defaultSemaphore;
        public SemaphoresNamed()
        {
            sempahores = new ConcurrentDictionary<string, SemaphoreSlim>();
            defaultSemaphore = RandomEx.NextBytesSecure(64).ToHexString();
        }

        public SemaphoreSlim GetLocker(string name = null)
        {
            if (name == null)
                name = defaultSemaphore;

            return locker.Lock(() =>
            {
                foreach(var k in sempahores.Keys)
                    if (!sempahores[k].IsLocked())
                        sempahores.TryRemove(k, out var s);

                if (!sempahores.ContainsKey(name))
                    sempahores.TryAdd(name, new SemaphoreSlim(1, 1));

                return sempahores[name];
            });
        }
    }
}
