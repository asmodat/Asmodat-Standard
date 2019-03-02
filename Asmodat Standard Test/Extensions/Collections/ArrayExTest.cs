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

        [Test]
        public void TakeLastWithRotation()
        {
            var a1 = Enumerable.Repeat(1, 5).ToArray();
            var a2 = Enumerable.Repeat(2, 5).ToArray();

            var r = ArrayEx.TakeLastWithRotation(new [] { 0, 1, 2, 3, 4, 5 }, 5, 2);
            CollectionAssert.AreEqual(new [] { 3, 4, 5, 0, 1 }, r);

            r = ArrayEx.TakeLastWithRotation(new[] { 0, 1, 2, 3, 4, 5 }, 2, 0);
            CollectionAssert.AreEqual(new[] { 4, 5 }, r);

            r = ArrayEx.TakeLastWithRotation(new[] { 0, 1, 2, 3, 4, 5 }, 6, 0);
            CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4, 5 }, r);

            r = ArrayEx.TakeLastWithRotation(new[] { 0, 1, 2, 3, 4, 5 }, 2, 3);
            CollectionAssert.AreEqual(new[] { 1, 2 }, r);

            r = ArrayEx.TakeLastWithRotation(new[] { 0, 1, 2, 3, 4, 5 }, 1, 5);
            CollectionAssert.AreEqual(new[] { 4 }, r);
        }
    }
}
