using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions.Cryptography
{
    public static class AesEx
    {
        public static bool IsValidKeySize(this CipherMode mode, int size)
        {
            using (var aes = new AesManaged())
            {
                aes.Mode = mode;
                return aes.ValidKeySize(size);
            }
        }

        public static AesSecret ToAesSecret(this string secret, 
            CipherMode mode = CipherMode.CBC, 
            PaddingMode padding = PaddingMode.PKCS7, byte[] ivLast = null)
        {
            if (secret.IsNullOrEmpty())
                throw new ArgumentException($"{nameof(secret)} can't be bull or empty");

            var shaSecret = secret.SHA256();
            var key = shaSecret.Take(32).ToArray();
            var iv = (ivLast ?? shaSecret).SHA256().Take(16).ToArray();
            return new AesSecret(key, iv, mode, padding);
        }

        public static async Task<string> AESB64Encrypt(this string s, string password, Encoding encoding = null)
        {
            s = s.Base64Encode();
            var bufferSize = 8*1024;
            var parts = new List<string>();

            byte[] iv = null;
            while (s.Length > 0)
            {
                var part = s.Length > bufferSize ? s.Substring(0, bufferSize) : s;
                var r = await AESB64EncryptPart(part, password, ivLast: iv, encoding: encoding);
                parts.Add(r.e);
                iv = r.iv;
                s = s.Length > bufferSize ? s.Substring(bufferSize, s.Length - bufferSize) : "";
            } 

            var jParts = parts.ToArray().JsonSerialize(formatting: Newtonsoft.Json.Formatting.None);
            return jParts.Base64Encode();
        }

        private static async Task<(string e, byte[] iv)> AESB64EncryptPart(this string s, string password, byte[] ivLast, Encoding encoding = null)
        {
            var secret = password.ToAesSecret(padding: PaddingMode.None, ivLast: ivLast);
            s = $"{s}#_______".Base64Encode();
            var input = s.ToMemoryStream(encoding: encoding);
            var output = await AES.EncryptAsync(input, secret);
            var outArr = output.ToArray();
            return (outArr.ToBase64String(), secret.IV);
        }

        public static async Task<string> AESB64Decrypt(this string e, string password, Encoding encoding = null)
        {
            var jArr = e.Base64Decode();
            var parts = jArr.JsonDeserialize<string[]>();
            var result = "";
            byte[] iv = null;
            foreach (var part in parts)
            {
                var r = await AESB64DecryptPart(part, password, iv, encoding: encoding);
                result += r.d;
                iv = r.iv;
            }

            return result.Base64Decode();
        }

        private static async Task<(string d, byte[] iv)> AESB64DecryptPart(this string e, string password, byte[] ivLast, Encoding encoding = null)
        {
            var secret = password.ToAesSecret(padding: PaddingMode.None, ivLast: ivLast);
            var inArr = e.FromBase64String();
            var @in = new MemoryStream(inArr);
            var @out = await AES.DecryptAsync(@in, secret);
            var s = (encoding ?? Encoding.UTF8).GetString(@out.ToArray());
            return (s.Base64Decode().SplitByLast('#')[0], secret.IV);
        }
    }
}
