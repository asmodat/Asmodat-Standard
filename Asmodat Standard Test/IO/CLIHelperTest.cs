using AsmodatStandard.IO;
using NUnit.Framework;

namespace AsmodatStandardTest.IO
{
    [TestFixture]
    public class CLIHelperTest
    {
        [Test]
        public void GetNamedArgumentsTest()
        {
            var args = new string[] {
                "-h",
                "--help",
                "--foo1=bar",
                "-foo2='bar bar'",
                "--foo3=\" bar bar bar \""
            };

            var nArgs = CLIHelper.GetNamedArguments(args);

            Assert.Contains("h", nArgs.Keys);
            Assert.Contains("help", nArgs.Keys);
            Assert.Contains("foo1", nArgs.Keys);
            Assert.Contains("foo2", nArgs.Keys);
            Assert.Contains("foo3", nArgs.Keys);

            Assert.AreEqual("bar", nArgs["foo1"]);
            Assert.AreEqual("bar bar", nArgs["foo2"]);
            Assert.AreEqual(" bar bar bar ", nArgs["foo3"]);
        }
    }
}
