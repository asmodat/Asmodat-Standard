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
    public class TickTimeoutTest
    {
        [Test]
        public void Test()
        {
            var tt = TickTimeoutEx.StartNew(5000);
            var sw = Stopwatch.StartNew();
            
            do
            {
                Thread.Sleep(10);
            } while (sw.ElapsedMilliseconds < 11000 && !tt.IsTriggered);

            var span = tt.Span;
            Assert.GreaterOrEqual(span, 5000);
            Assert.LessOrEqual(span, 5000*2);
        }
    }
}
