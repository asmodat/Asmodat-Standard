using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Cryptography;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class AESTest
    {
        [Test]
        public async Task AESStringEncryptDecrypt()
        {
            for (int i = 0; i < 100; i++)
            {
                var pass = RandomEx.NextString(RandomEx.Next(1, 1024), char.MinValue, char.MaxValue)
                    .Base64Encode().Base64Decode();
                var s = RandomEx.NextString(RandomEx.Next(1, 1024 * 64), char.MinValue, char.MaxValue)
                    .Base64Encode().Base64Decode();

                var e = await s.AESB64Encrypt(pass);
                var d = await e.AESB64Decrypt(pass);

                Assert.AreEqual(d, s);
                Assert.AreNotEqual(s, e);
            }
        }

        [Test]
        public async Task AESEncryptDecrypt()
        {
            var dir = Directory.GetCurrentDirectory();
            var zip = Path.Combine(dir, "Cryptography", "HashDirectory.zip").ToFileInfo();
            var zipEncrypted = Path.Combine(dir, "Cryptography", "HashDirectoryAES256_Encrypted").ToFileInfo();
            var zipDecrypted = Path.Combine(dir, "Cryptography", "HashDirectoryAES256_Decrypted").ToFileInfo();
            var fileSecret = Path.Combine(dir, "Cryptography", "AesSecret256.aess").ToFileInfo();

            zipEncrypted.TryDelete();
            zipDecrypted.TryDelete();

            fileSecret.CreateAesSecret();
            var secret = fileSecret.LoadAesSecret();

            var md5 = zip.MD5();
            await zip.EncryptAsync(secret, zipEncrypted);
            var md5Encrypted = zipEncrypted.MD5();

            CollectionAssert.AreNotEqual(md5Encrypted, md5);

            await zipEncrypted.DecryptAsync(secret, zipDecrypted);
            var md5Decrypted = zipDecrypted.MD5();

            CollectionAssert.AreEqual(md5, md5Decrypted);
        }
        
    }
}
