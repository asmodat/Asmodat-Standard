using AsmodatStandard.IO;
using NUnit.Framework;
using AsmodatStandard.Extensions.IO;
using AsmodatStandard.Extensions;

namespace AsmodatStandardTest.IO
{
    [TestFixture]
    public class PathExTest
    {
        [Test]
        public void CombineTest()
        {
            Assert.AreEqual(PathEx.Combine("\\", "a", "b"), "\\a\\b");
            Assert.AreEqual(PathEx.Combine(" \\", " a", "b"), "\\a\\b");
            Assert.AreEqual(PathEx.Combine("\\ ", "a", " b"), "\\a\\b");
            Assert.AreEqual(PathEx.Combine(" \\ ", " a c ", " b "), "\\a c\\b");
            Assert.AreEqual(PathEx.Combine("a", "b"), "a\\b");
            Assert.AreEqual(PathEx.Combine("a ", "b"), "a\\b");
            Assert.AreEqual(PathEx.Combine("a", "b "), "a\\b");
            Assert.AreEqual(PathEx.Combine(" a", " b "), "a\\b");
            Assert.AreEqual(PathEx.Combine("a", "b", "/" , "c"), "a\\b\\c");
            Assert.AreEqual(PathEx.Combine("a", "b", "\\", "c"), "a\\b\\c");
            Assert.AreEqual(PathEx.Combine(isWindowsRuntime: false, "/a/b/c/d/e/f", "/g", "h"), "/a/b/c/d/e/f/g/h");
            Assert.AreEqual(PathEx.Combine(isWindowsRuntime: false, "/a/b/////c/d/e/f", "/   g", "//h"), "/a/b/c/d/e/f/g/h");
            Assert.AreEqual(PathEx.Combine(isWindowsRuntime: false, "a/b/c/d/e/f", "/g", "h"), "a/b/c/d/e/f/g/h");
        }
    }
}
