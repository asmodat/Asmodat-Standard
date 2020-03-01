using NUnit.Framework;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Threading;
using System.Diagnostics;
using System;
using AsmodatStandard.Threading;

namespace AsmodatStandardTest.Extensions.Threading
{
    [TestFixture]
    public class TaskExTest
    {

        [Test]
        public void TimeoutTest()
        {
            async Task<bool> DelayReturn(int msSleep, bool throwException = false)
            {
                await Task.Delay(msSleep);
                if (throwException)
                    throw new SuccessException("DelayTest Exception");
                return true;
            }

            async Task DelayNoReturn(int msSleep, bool throwException = false)
            {
                await Task.Delay(msSleep);
                if (throwException)
                    throw new SuccessException("DelayTest Exception");
            }

            Assert.ThrowsAsync<TimeoutException>(() => DelayNoReturn(2000).Timeout(msTimeout: 1000));
            Assert.ThrowsAsync<TimeoutException>(() => DelayReturn(2000).Timeout(msTimeout: 1000));

            Assert.DoesNotThrowAsync(() => DelayNoReturn(500).Timeout(msTimeout: 1000));
            Assert.DoesNotThrowAsync(() => DelayReturn(250, false).Timeout(msTimeout: 1000));

            Assert.ThrowsAsync<SuccessException>(() => DelayNoReturn(500, true).Timeout(msTimeout: 1000));
            Assert.ThrowsAsync<SuccessException>(() => DelayReturn(500, true).Timeout(msTimeout: 1000));
        }
    }
}
