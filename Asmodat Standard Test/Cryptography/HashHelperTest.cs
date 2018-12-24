using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class HashHelperTest
    {
        [Test]
        public void IsValidSHA256Hex()
        {
            Assert.IsTrue("7f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069".IsValidSHA256Hex());
            Assert.IsTrue("7f83b1657ff1fc53b92dc18148a1d65dfC2d4b1fa3d677284addd200126d9069".IsValidSHA256Hex());
            Assert.IsFalse("87f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069".IsValidSHA256Hex());
            Assert.IsFalse("f83b1657ff1fc53b92dc18148a1d65dfc2d4b1fa3d677284addd200126d9069".IsValidSHA256Hex());
            Assert.IsFalse("f83b1657ff1fc53b92dc18148a1d65dfc2dg4b1fa3d677284addd200126d9069".IsValidSHA256Hex());
            Assert.IsFalse("f83b1657ff1fc53b92dc18148a1d65dfc2d-4b1fa3d677284addd200126d9069".IsValidSHA256Hex());
            Assert.IsFalse("".IsValidSHA256Hex());
            Assert.IsFalse((null as string).IsValidSHA256Hex());
        }

        [Test]
        public async Task HashDirectorySHA256()
        {
            var dir = Directory.GetCurrentDirectory();
            var wkd = Path.Combine(dir, "Cryptography", "HashDirectorySHA256").ToDirectoryInfo();
            var zip = Path.Combine(dir, "Cryptography", "HashDirectory.zip").ToFileInfo();

            if (wkd.Exists)
                wkd.Delete(recursive: true);

            zip.UnZip(wkd);

            string hash = "832dd624016199838c0a3be552b4bb7e00254d8ea394226eb2881308a06902ef";
            string hashRootLess = "500a49610e9db4a23976e8efe8b4ffe35af5bbdd1b2bfe47054aa351f819afbb";

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

            string hash = "1f49827d48429d5c30d5ef4be4453a76";
            string hashRootLess = "ee24db1a413d193cdb4b480086732c4a";

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
