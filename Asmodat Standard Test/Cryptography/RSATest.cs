using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using AsmodatStandard.Extensions.Collections;
using System;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class RSATest
    {
        DirectoryInfo WorkingDirectory;

        public RSATest()
        {
            var dir = Directory.GetCurrentDirectory();
            WorkingDirectory = Path.Combine(dir, "AsmodatStandardTest", "Cryptography", "RSATest").ToDirectoryInfo();

            if (WorkingDirectory.Exists)
                WorkingDirectory.Delete(recursive: true);

            WorkingDirectory.Create();
        }

        [Test]
        public void GenerateTest()
        {
            var ackp = RSA.GenerateKey(1024);
            var pemPK = RSA.GeneratePemKeyPair(1024);

            Assert.IsNotNull(ackp);
            Assert.IsNotNull(pemPK);
            Assert.IsNotNull(ackp.Public);
            Assert.IsNotNull(ackp.Private);
            Assert.IsNotNull(pemPK.Public);
            Assert.IsNotNull(pemPK.Private);
        }

        [Test]
        public void SignAndVerifyTest()
        {
            var ackp = RSA.GenerateKey(2048);
            var ackp2 = RSA.GenerateKey(2048);

            foreach (var algo in EnumEx.ToArray<RSA.Algorithm>())
            {
                var size = RandomEx.Next(1 * 1024, 2 * 1024);
                var arr1 = RandomEx.NextBytes(size);
                var arr2 = arr1.Merge(new byte[1] { 0 });

                var sig1 = RSA.Sign(arr1, ackp.Private, algo);
                var sig2 = RSA.Sign(arr2, ackp.Private, algo);
                var sig3 = RSA.Sign(arr1, ackp2.Private, algo);
                var sig4 = RSA.Sign(arr2, ackp2.Private, algo);

                Assert.AreNotEqual(sig1, sig2);
                Assert.AreNotEqual(sig1, sig3);
                Assert.AreNotEqual(sig1, sig4);
                Assert.AreNotEqual(sig2, sig3);
                Assert.AreNotEqual(sig2, sig4);
                Assert.AreNotEqual(sig3, sig4);

                Assert.IsTrue(RSA.Verify(arr1, sig1, ackp.Public, algo));
                Assert.IsTrue(RSA.Verify(arr2, sig2, ackp.Public, algo));
                Assert.IsTrue(RSA.Verify(arr1, sig3, ackp2.Public, algo));
                Assert.IsTrue(RSA.Verify(arr2, sig4, ackp2.Public, algo));
                Assert.IsFalse(RSA.Verify(arr2, sig1, ackp.Public, algo));
                Assert.IsFalse(RSA.Verify(arr1, sig2, ackp.Public, algo));
                Assert.IsFalse(RSA.Verify(arr2, sig3, ackp2.Public, algo));
                Assert.IsFalse(RSA.Verify(arr1, sig4, ackp2.Public, algo));
            }
        }

        [Test]
        public void SignAndVerifyPEMTest()
        {
            var PEMKP = RSA.GeneratePemKeyPair(1024);
            var arr1 = RandomEx.NextBytes(RandomEx.Next(4096, 256 * 1024));
            var arr2 = RandomEx.NextBytes(arr1.Length);
            var algo = RSA.Algorithm.SHA256WITHRSA;

            var basePath = Path.Combine(WorkingDirectory.FullName, Guid.NewGuid().ToString());
            var file = $"{basePath}.test".ToFileInfo();
            var file2 = $"{basePath}.test2".ToFileInfo();
            var filePub = $"{basePath}.public.pem".ToFileInfo();
            var filePrv = $"{basePath}.private.pem".ToFileInfo();
            var fileSig = $"{basePath}.rsa.sig".ToFileInfo();

            file.WriteAllBytes(arr1);
            file2.WriteAllBytes(arr2);
            filePub.WriteAllText(PEMKP.Public);
            filePrv.WriteAllText(PEMKP.Private);

            var sig = RSA.SignWithPemKey(arr1, filePrv, algo);
            var sig0 = RSA.SignWithPemKey(file, filePrv, algo);

            Assert.AreEqual(sig, sig0);

            fileSig.WriteAllBytes(sig);

            Assert.IsTrue(RSA.VerifyWithPemKey(arr1, sig, filePub, algo));
            Assert.IsTrue(RSA.VerifyWithPemKey(file, sig, filePub, algo));
            Assert.IsTrue(RSA.VerifyWithPemKey(file, fileSig, filePub, algo));

            Assert.IsFalse(RSA.VerifyWithPemKey(arr2, sig, filePub, algo));
            Assert.IsFalse(RSA.VerifyWithPemKey(file2, sig, filePub, algo));
            Assert.IsFalse(RSA.VerifyWithPemKey(file2, fileSig, filePub, algo));
        }

    }
}
