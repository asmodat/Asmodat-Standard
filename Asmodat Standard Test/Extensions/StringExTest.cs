using NUnit.Framework;
using AsmodatStandard.Extensions;

namespace AsmodatStandardTest.Extensions.StringExTests
{
    [TestFixture]
    public class StringExTest
    {
        [Test]
        public void GZipTest()
        {
            void Test(string s) 
                => Assert.AreEqual(s, s.GZip().UnGZip());

            Test("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            Test("babacc");
            Test("babagggggggggggggggggggghhhhhhhhhhhhhhhcc");
            Test("abcdefghijkabcdefghijkabcdefghijkabcdefghijkab0987654");
        }

        [Test]
        public void IsWildcardMatchTest()
        {
            Assert.IsTrue("".IsWildcardMatch(""));
            Assert.IsTrue("1".IsWildcardMatch("?"));
            Assert.IsTrue("".IsWildcardMatch("*"));
            Assert.IsTrue("a".IsWildcardMatch("a*"));
            Assert.IsTrue("?a".IsWildcardMatch("\\?a*"));
            Assert.IsTrue("bla*a".IsWildcardMatch("bla\\*a*"));
            Assert.IsTrue("?*?".IsWildcardMatch("?*?"));
            Assert.IsTrue("?*a".IsWildcardMatch("\\?\\*?"));
            Assert.IsTrue("XYZ".IsWildcardMatch("???"));
            Assert.IsTrue("XYZ".IsWildcardMatch("?YZ"));
            Assert.IsTrue("XYZ".IsWildcardMatch("X?Z"));
            Assert.IsTrue("XYZ".IsWildcardMatch("*"));
            Assert.IsTrue("XYZ".IsWildcardMatch("*Z"));
            Assert.IsFalse("bla*a".IsWildcardMatch("ble\\*a*"));
            Assert.IsFalse("XYZ".IsWildcardMatch("Y*"));
            Assert.IsFalse("XYZ".IsWildcardMatch("?YX"));
            Assert.IsFalse("".IsWildcardMatch("?"));
            Assert.IsFalse("a".IsWildcardMatch("a?"));
        }

        [Test]
        public void EscapedSplitTest()
        {
            Assert.IsTrue("1,2,3".EscapedSplit(',').Length == 3);
            Assert.IsTrue("1,2\\,3".EscapedSplit(',').Length == 2);
            Assert.IsTrue("1\\,2\\,3".EscapedSplit(',').Length == 1);
            Assert.IsTrue(",,,".EscapedSplit(',').Length == 4);
        }
    }
}
