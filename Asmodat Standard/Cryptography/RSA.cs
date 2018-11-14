using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.IO;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace AsmodatStandard.Cryptography
{
    public class PEMKeyPair
    {
        public readonly string Public;
        public readonly string Private;

        public PEMKeyPair(string publicKey, string privateKey)
        {
            this.Public = publicKey;
            this.Private = privateKey;
        }
    }

    public static class RSA
    {
        /// <summary>
        /// https://github.com/bcgit/bc-csharp/blob/f18a2dbbc2c1b4277e24a2e51f09cac02eedf1f5/crypto/src/security/SignerUtilities.cs
        /// </summary>
        public enum Algorithm : int
        {
            MD2WITHRSA = 1,
            MD4WITHRSA,
            MD5WITHRSA,
            SHA1WITHRSA,
            SHA224WITHRSA,
            SHA256WITHRSA,
            SHA384WITHRSA,
            SHA512WITHRSA,
            PSSWITHRSA,
            SHA1WITHRSAANDMGF1,
            SHA224WITHRSAANDMGF1,
            SHA256WITHRSAANDMGF1,
            SHA384WITHRSAANDMGF1,
            SHA512WITHRSAANDMGF1,
            RIPEMD128WITHRSA,
            RIPEMD160WITHRSA,
            RIPEMD256WITHRSA
        }

        public static AsymmetricCipherKeyPair GenerateKey(int size)
        {
            var rkpg = new RsaKeyPairGenerator();
            var sr = new SecureRandom();
            var kgp = new KeyGenerationParameters(sr, size);
            rkpg.Init(kgp);
            return rkpg.GenerateKeyPair();
        }

        public static PEMKeyPair GeneratePemKeyPair(int size)
        {
            var keys = GenerateKey(size);
            string pub, prv;
            using (TextWriter tw = new StringWriter())
            {
                var pw = new PemWriter(tw);
                pw.WriteObject(keys.Public);
                pw.Writer.Flush();
                pub = tw.ToString();
            }

            using (TextWriter tw = new StringWriter())
            {
                var pw = new PemWriter(tw);
                pw.WriteObject(keys.Private);
                pw.Writer.Flush();
                prv = tw.ToString();
            }

            return new PEMKeyPair(publicKey: pub, privateKey: prv);
        }

        public static ICipherParameters PemToPublicCipherKeyParameter(string pem)
        {
            using (TextReader tr = new StringReader(pem))
            {
                var pw = new PemReader(tr);
                var obj = pw.ReadObject();
                return ((RsaKeyParameters)obj);
            }
        }

        public static ICipherParameters PemToPrivateCipherKeyParameter(string pem)
        {
            using (TextReader tr = new StringReader(pem))
            {
                var pw = new PemReader(tr);
                var obj = pw.ReadObject();
                return ((AsymmetricCipherKeyPair)obj).Private;
            }
        }

        public static RSAParameters PemToRSAParameters(string pem)
        {
            if (pem.Contains("private", CompareOptions.IgnoreCase))
            {
                var rsaPCKP = PemToPrivateCipherKeyParameter(pem) as RsaPrivateCrtKeyParameters;
                return DotNetUtilities.ToRSAParameters(rsaPCKP);
            }
            if (pem.Contains("public", CompareOptions.IgnoreCase))
            {
                var rsaKP = PemToPublicCipherKeyParameter(pem) as RsaKeyParameters;
                var rsaCSP = new RSACryptoServiceProvider();
                return new RSAParameters()
                {

                    Modulus = rsaKP.Modulus.ToByteArrayUnsigned(),
                    Exponent = rsaKP.Exponent.ToByteArrayUnsigned()
                };
            }
            else
                throw new Exception("Failed RSA Parameters extraction, could not determine if PEM file is a PUBLIC or PRIVATE key.");
        }

        public static RsaSecurityKey ToRsaSecurityKey(this RSAParameters rsaParams)
        {
            var rsaCSP = new RSACryptoServiceProvider();
            rsaCSP.ImportParameters(rsaParams);
            return new RsaSecurityKey(rsaCSP);
        }

        public static RsaSecurityKey PemToRsaSecurityKey(string pem) => PemToRSAParameters(pem).ToRsaSecurityKey();

        public static byte[] Sign(byte[] input, ICipherParameters privateKey, Algorithm algorithm)
        {
            var su = SignerUtilities.GetSigner(algorithm.ToString());
            su.Init(forSigning: true, privateKey);
            su.BlockUpdate(input, 0, input.Length);
            return su.GenerateSignature();
        }

        public static byte[] SignWithPemKey(byte[] input, string pem, Algorithm algorithm)
        {
            var key = PemToPrivateCipherKeyParameter(pem);
            return Sign(input, key, algorithm);
        }

        public static byte[] SignWithPemKey(byte[] input, FileInfo pem, Algorithm algorithm)
        {
            if (!pem.Exists || pem.Length <= 64)
                throw new Exception($"PEM Private Key was not found within: '{pem?.FullName}'.");

            var keyContent = pem.ReadAllText();

            if(!keyContent.Contains("private", CompareOptions.IgnoreCase))
                throw new Exception($"Key '{pem?.FullName}' is not a private key.");

            return SignWithPemKey(input, keyContent, algorithm);
        }

        public static byte[] SignWithPemKey(FileInfo input, FileInfo pem, Algorithm algorithm)
        {
            if (!input.Exists)
                throw new Exception($"Input file was not found or is empty: '{input?.FullName}'.");

            return SignWithPemKey(input.ReadAllBytes(), pem, algorithm);
        }

        public static bool Verify(byte[] input, byte[] signature, ICipherParameters publicKey, Algorithm algorithm)
        {
            var su = SignerUtilities.GetSigner(algorithm.ToString());
            su.Init(forSigning: false, publicKey);
            su.BlockUpdate(input, 0, input.Length);
            return su.VerifySignature(signature);
        }

        public static bool VerifyWithPemKey(byte[] input, byte[] signature, string pem, Algorithm algorithm)
        {
            var key = PemToPublicCipherKeyParameter(pem);
            return Verify(input, signature, key, algorithm);
        }

        public static bool VerifyWithPemKey(byte[] input, byte[] signature, FileInfo pem, Algorithm algorithm)
        {
            if (!pem.Exists || pem.Length <= 64)
                throw new Exception($"PEM Public Key was not found within: '{pem?.FullName}'.");

            var keyContent = pem.ReadAllText();

            if (!keyContent.Contains("public", CompareOptions.IgnoreCase))
                throw new Exception($"Key '{pem?.FullName}' is not a public key.");

            return VerifyWithPemKey(input, signature, keyContent, algorithm);
        }

        public static bool VerifyWithPemKey(FileInfo input, byte[] signature, FileInfo pem, Algorithm algorithm)
        {
            if (!input.Exists)
                throw new Exception($"Input file was not found: '{input?.FullName}'.");

            return VerifyWithPemKey(input.ReadAllBytes(), signature, pem, algorithm);
        }

        public static bool VerifyWithPemKey(FileInfo input, FileInfo signature, FileInfo pem, Algorithm algorithm)
        {
            if (!signature.Exists)
                throw new Exception($"Signature file was not found: '{signature?.FullName}'.");

            return VerifyWithPemKey(input.ReadAllBytes(), signature.ReadAllBytes(), pem, algorithm);
        }
    }
}
