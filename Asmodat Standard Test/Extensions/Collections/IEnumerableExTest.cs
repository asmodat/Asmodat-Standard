using AsmodatStandard.Extensions.Collections;
using NUnit.Framework;
using System.Linq;

namespace AsmodatStandardTest.Extensions.Collections
{
    internal class DistinctByTestClass
    {
        public DistinctByTestClass(string a, string b)
        {
            this.A = a;
            this.B = b;
        }

        public string A;
        public string B;
    }

    [TestFixture]
    public class IEnumerableExTest
    {
        [Test]
        public void Slice()
        {
            var a0 = new int[0];
            var s0 = a0.Batch(1)?.ToArray();
            Assert.AreEqual(0, s0.Count());

            var a1 = Enumerable.Repeat(1, 10).ToArray();
            var s1 = a1.Batch(1).ToArray();
            Assert.AreEqual(10, s1.Count());

            var a2 = Enumerable.Range(1, 11).ToArray();
            var s2 = a2.Batch(2).ToArray();
            Assert.AreEqual(s2.Count(), 6);
            Assert.AreEqual(s2.First().Count(), 2);
            Assert.AreEqual(s2.Last().Count(), 1);
        }

        [Test]
        public void DistinctByTest()
        {
            var arr = new DistinctByTestClass[] {
                new DistinctByTestClass("a", "b"),
                new DistinctByTestClass("a", "c"),
                new DistinctByTestClass("a", "d"),
                new DistinctByTestClass("a", "e"),
                new DistinctByTestClass("a", "f"),
            };

            Assert.AreEqual(arr.DistinctBy(x => x.A).Count(), 1);
            Assert.AreEqual(arr.DistinctBy(x => x.B).Count(), 5);

            arr[3].A = "b";
            Assert.AreEqual(arr.DistinctBy(x => x.A).Count(), 2);

            arr[4].A = null;
            Assert.AreEqual(arr.DistinctBy(x => x.A).Count(), 3);

            arr[2].A = null;
            Assert.AreEqual(arr.DistinctBy(x => x.A).Count(), 3);

            arr = new DistinctByTestClass[0];
            Assert.AreEqual(arr.DistinctBy(x => x.A).Count(), 0);
        }
    }
}
