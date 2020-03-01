using NUnit.Framework;
using AsmodatStandard.Extensions;

namespace AsmodatStandardTest.Extensions.StringExTests
{
    [TestFixture]
    public class StringExTest
    {
        [Test]
        public void Base61A()
        {
            void TestBase(string e, string d)
            {
                var dEncode = d.Base61AEncode();
                var eDecode = e.Base61ADecode();
                Assert.IsTrue(e.IsBase61A());
                Assert.AreEqual(e, dEncode);
                Assert.AreEqual(eDecode, d);
            }

            Assert.IsFalse(((string)null).IsBase61A());

            TestBase("_A_","");
            TestBase("A_AAAA", "AAAA");
            TestBase("5YO55_", "價");

            for (int i = 1; i < 1024; i++)
            {
                var r = RandomEx.Next(1, i);
                var s = RandomEx.NextString(r, char.MinValue, char.MaxValue);
                var e = s.Base61AEncode();
                string d = null;
                bool isBase = false;
                try
                {
                    isBase = e.IsBase61A();
                    d = e.Base61ADecode();
                }
                catch
                {
                    throw new System.Exception($"Failed to encode {s}");
                }

                Assert.IsTrue(isBase);

                var ed = s.Base64Encode().Base64Decode();
                Assert.AreEqual(ed, d);
            }
        }

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
