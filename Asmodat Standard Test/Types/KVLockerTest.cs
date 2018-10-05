using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Threading;
using System.Diagnostics;
using System;
using AsmodatStandard.Types;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.Types;

namespace AsmodatStandardTest.Types
{
    [TestFixture]
    public class KVLockerTest
    {
        [Test]
        public void Test()
        {
            var kvl = new KVLocker();

            bool lockingStarted = false;

            Assert.IsFalse(kvl.GetLock("aaa").IsLocked());

            var thred1 = new Thread(() => {
                kvl.GetLock("aaa").Lock(() => {
                    lockingStarted = true;

                    while(lockingStarted)
                        Thread.Sleep(100);
                });
            });

            thred1.Start();

            while(!lockingStarted)
                Thread.Sleep(100);

            Assert.IsTrue(kvl.GetLock("aaa").IsLocked());
            lockingStarted = false;

            thred1.Join();

            Assert.IsFalse(kvl.GetLock("aaa").IsLocked());
            Assert.IsFalse(kvl.GetLock("bbb").IsLocked());
        }
    }
}
