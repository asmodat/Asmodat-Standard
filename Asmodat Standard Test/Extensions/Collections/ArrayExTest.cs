using AsmodatStandard.Extensions.Collections;
using NUnit.Framework;
using System.Linq;

namespace AsmodatStandardTest.Extensions.Collections
{
    [TestFixture]
    public class ArrayExTest
    {
        [Test]
        public void Merge()
        {
            var a1 = Enumerable.Repeat(1, 5).ToArray();
            var a2 = Enumerable.Repeat(2, 5).ToArray();

            var expected = new int[] { 1,1,1,1,1,2,2,2,2,2 };

            var r1 = ArrayEx.Merge(new int[2][] { a1, a2 });
            var r2 = ArrayEx.Merge(a1, a2);
            var r3 = ArrayEx.Merge(a1, a2);

            CollectionAssert.AreEqual(expected, r1);
            CollectionAssert.AreEqual(expected, r2);
            CollectionAssert.AreEqual(expected, r3);
        }
    }
}
