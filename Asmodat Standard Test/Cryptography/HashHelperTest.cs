using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;

namespace AsmodatStandard.Cryptography.HashHelperTest
{
    [TestFixture]
    public class HashHelperTest
    {
        [Test]
        public async Task HashDirectory()
        {
            var dir = Directory.GetCurrentDirectory();
            var wkd = Path.Combine(dir, "Cryptography", "HashDirectory").ToDirectoryInfo();
            var zip = Path.Combine(dir, "Cryptography", "HashDirectory.zip").ToFileInfo();

            if (wkd.Exists)
                wkd.Delete(recursive: true);

            zip.UnZip(wkd);

            string hash = "b8f26fcd5b05b1c8e091e46538f553791a58aa3834779e6424baa5e92109fd52";
            string hashRootLess = "f43dc22950545061fb493c82947dc0d0fc3bc64d624daec7140263a471a0c16c";

            wkd.Refresh();
            DirectoryAssert.Exists(wkd);

            var hash0 = (await wkd.SHA256(excludeRootName: true, recursive: true)).ToHexString();
            Assert.AreEqual(hashRootLess, hash0);

            var hash1 = (await wkd.SHA256(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreEqual(hash, hash1);

            var mdir = wkd.Combine("empty", "test").ToDirectoryInfo();
            mdir.Create();

            var hash2 = (await wkd.SHA256(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreNotEqual(hash, hash2);
            mdir.Delete();

            using (var fs = wkd.Combine("level1", "level2", "level3", "u.f3").ToFileInfo().OpenWrite())
                fs.WriteByte(0);

            var hash3 = (await wkd.SHA256(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreNotEqual(hash, hash3);
            Assert.AreNotEqual(hash2, hash3);

            var wkd2 = Path.Combine(dir, "Cryptography", "tmp", "HashDirectory").ToDirectoryInfo();

            if (wkd2.Exists)
                wkd2.Delete(recursive: true);

            zip.UnZip(wkd2);

            var hash4 = (await wkd2.SHA256(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreEqual(hash, hash4);

            var hash5 = (await wkd2.SHA256(excludeRootName: true, recursive: true)).ToHexString();
            Assert.AreNotEqual(hash, hash5);

            Assert.AreEqual(hashRootLess, hash5);
        }
    }
}
