using NUnit.Framework;
using System.IO;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.IO;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Cryptography;
using AsmodatStandard.Extensions.Collections;
using System;
using System.Security.Claims;
using System.Threading;

namespace AsmodatStandardTest.Cryptography
{
    [TestFixture]
    public class JwtHelperTest
    {
        DirectoryInfo WorkingDirectory;

        public JwtHelperTest()
        {
            var dir = Directory.GetCurrentDirectory();
            WorkingDirectory = Path.Combine(dir, "AsmodatStandardTest", "Cryptography", "JwtHelperTest").ToDirectoryInfo();

            if (WorkingDirectory.Exists)
                WorkingDirectory.Delete(recursive: true);

            WorkingDirectory.Create();
        }

        [Test]
        public void TokenTest()
        {
            var kp1 = RSA.GeneratePemKeyPair(2048);
            var kp2 = RSA.GeneratePemKeyPair(2048);

            var issuer = "issuer-1";
            var audience = "audience-1";
            var issued = DateTime.UtcNow;

            var token1 = JwtHelper.GenerateTokenPEMRS256(
                issuer: issuer,
                audience: audience,
                new[] { new Claim(ClaimTypes.Name, "user-1") },
                privatePemKey: kp1.Private,
                expires: issued.AddHours(1),
                issuedAt: issued,
                notBefore: issued);

            var tokenExpired = JwtHelper.GenerateTokenPEMRS256(
                issuer: issuer,
                audience: audience,
                new[] { new Claim(ClaimTypes.Name, "user-1") },
                privatePemKey: kp1.Private,
                issuedAt: issued,
                expires: issued.AddSeconds(1),
                notBefore: issued);

            Assert.IsTrue(JwtHelper.VerifyTokenPEMRSA(token1, kp1.Public));
            Assert.IsTrue(JwtHelper.VerifyTokenPEMRSA(token1, kp1.Public, validIssuers:
                new[] { issuer }));
           
            Assert.IsFalse(JwtHelper.VerifyTokenPEMRSA(token1, kp1.Public, validIssuers:
                new[] { issuer + " " }));

            while (DateTime.UtcNow <= issued.AddSeconds(1))
                Thread.Sleep(100);

            Assert.IsTrue(JwtHelper.VerifyTokenPEMRSA(tokenExpired, kp1.Public));
            Assert.IsFalse(JwtHelper.VerifyTokenPEMRSA(
                tokenExpired, 
                kp1.Public,
                verifyIfNotExpired: true));
            Assert.IsFalse(JwtHelper.VerifyTokenPEMRSA(token1, kp2.Public));
        }
    }
}
