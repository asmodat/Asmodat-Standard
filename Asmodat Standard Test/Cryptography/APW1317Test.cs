using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class APW1317Test
    {
        [Test]
        public void CreateAndVerifyManual()
        {
            var dificulty = 10;
            var pass = "bla";
            var secret = "4601ce0ccc7df21db537d76c464c96a8ef1c6bf2195a6d762f40d3a4607f67b7";  

            Assert.IsTrue(APW1317.Verify(pass, secret, dificulty));
            Assert.IsFalse(APW1317.Verify("ble", secret, dificulty));
        }

        [Test]
        public void CreateAndVerifyRandom()
        {
            for (int i = 1; i < 1000; i++)
            {
                var dificulty = i;
                var pass = RandomEx.NextStringASCI(i);
                var passFalse = RandomEx.NextStringASCI(i);
                var secret = APW1317.Generate(pass, dificulty);
                Assert.IsTrue(APW1317.Verify(pass, secret, dificulty));
                Assert.IsFalse(APW1317.Verify(passFalse, secret, dificulty));
            }
        }
    }
}
