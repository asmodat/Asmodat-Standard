using NUnit.Framework;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandardTest.Extensions.StringExTests
{
    [TestFixture]
    public class HttpHelperTest
    {
        [Test]
        public void UriEncodeDecodeRandomTest()
        {
            for (int i = 0; i < 100; i++)
            {
                var initial = RandomEx.NextStringASCI(RandomEx.Next(1, 100));
                var encoded = initial.UriEncode();

                if (initial.ContainsAny("%", " ", "+", "$", "?", "\\", "~", ":"))
                    Assert.AreNotEqual(initial, encoded);

                var decoded = encoded.UriDecode();
                Assert.AreEqual(initial, decoded);
            }
        }
    }
}
