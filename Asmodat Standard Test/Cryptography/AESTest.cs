using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class AESTest
    {
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
