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
        public async Task HashDirectorySHA256()
        {
            var dir = Directory.GetCurrentDirectory();
            var wkd = Path.Combine(dir, "Cryptography", "HashDirectorySHA256").ToDirectoryInfo();
            var zip = Path.Combine(dir, "Cryptography", "HashDirectory.zip").ToFileInfo();

            if (wkd.Exists)
                wkd.Delete(recursive: true);

            zip.UnZip(wkd);

            string hash = "e8e2730bc929465dc1bd75e79597de3acb7c34152a96aac0ee481b19f470eb1b";
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

            var wkd2 = Path.Combine(dir, "Cryptography", "tmp", "HashDirectorySHA256").ToDirectoryInfo();

            if (wkd2.Exists)
                wkd2.Delete(recursive: true);

            zip.UnZip(wkd2);

            var hash4 = (await wkd2.SHA256(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreEqual(hash, hash4);

            var hash5 = (await wkd2.SHA256(excludeRootName: true, recursive: true)).ToHexString();
            Assert.AreNotEqual(hash, hash5);

            Assert.AreEqual(hashRootLess, hash5);
        }

        [Test]
        public async Task HashDirectoryMD5()
        {
            var dir = Directory.GetCurrentDirectory();
            var wkd = Path.Combine(dir, "Cryptography", "HashDirectoryMD5").ToDirectoryInfo();
            var zip = Path.Combine(dir, "Cryptography", "HashDirectory.zip").ToFileInfo();

            if (wkd.Exists)
                wkd.Delete(recursive: true);

            zip.UnZip(wkd);

            string hash = "4fc5c30cf1e82d5d2b8fc135aef06a0c";
            string hashRootLess = "fd7d410918e112e6393c71d9d5477dc4";

            wkd.Refresh();
            DirectoryAssert.Exists(wkd);

            var hash0 = (await wkd.MD5(excludeRootName: true, recursive: true)).ToHexString();
            Assert.AreEqual(hashRootLess, hash0);

            var hash1 = (await wkd.MD5(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreEqual(hash, hash1);

            var mdir = wkd.Combine("empty", "test").ToDirectoryInfo();
            mdir.Create();

            var hash2 = (await wkd.MD5(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreNotEqual(hash, hash2);
            mdir.Delete();

            using (var fs = wkd.Combine("level1", "level2", "level3", "u.f3").ToFileInfo().OpenWrite())
                fs.WriteByte(0);

            var hash3 = (await wkd.MD5(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreNotEqual(hash, hash3);
            Assert.AreNotEqual(hash2, hash3);

            var wkd2 = Path.Combine(dir, "Cryptography", "tmp", "HashDirectoryMD5").ToDirectoryInfo();

            if (wkd2.Exists)
                wkd2.Delete(recursive: true);

            zip.UnZip(wkd2);

            var hash4 = (await wkd2.MD5(excludeRootName: false, recursive: true)).ToHexString();
            Assert.AreEqual(hash, hash4);

            var hash5 = (await wkd2.MD5(excludeRootName: true, recursive: true)).ToHexString();
            Assert.AreNotEqual(hash, hash5);

            Assert.AreEqual(hashRootLess, hash5);
        }
    }
}
