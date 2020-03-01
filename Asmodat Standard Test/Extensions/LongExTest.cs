using NUnit.Framework;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using System.Diagnostics;
using System.Threading;

namespace AsmodatStandardTest.Extensions.StringExTests
{
    [TestFixture]
    public class LongExTest
    {
        [Test]
        public void ToPrettyTimeSpan()
        {
            var sw = Stopwatch.StartNew();
            Thread.Sleep(2000);
            var pts = sw.ToPrettyTimeSpan();
            Assert.AreNotEqual("0 seconds", pts?.ToLower());
        }

    }
}
