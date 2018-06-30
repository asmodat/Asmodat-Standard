using AsmodatStandard.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandardTest.Extensions.RandomExTests
{
    [TestFixture]
    public class RandomExTest
    {
        [TestCase(100, 200, 50)]
        [TestCase(-100, 200, 100)]
        [TestCase(0, 1, 1)]
        [TestCase(0, 100, 100)]
        public void NextDistinctTest(int min, int max, int count)
        {
            var arr = RandomEx.NextDistinct(min, max, count);
            var distinct = arr.Distinct();
            Assert.AreEqual(distinct.Count(), count);

            foreach (var v in distinct)
                Assert.IsTrue(v < max && v >= min);
        }
    }
}
