using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Cryptography;
using Newtonsoft.Json;

namespace AsmodatStandard.Cryptography
{
    public class AesSecret
    {
        public AesSecret(byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            if (!mode.IsValidKeySize(key.Length * 8))
                throw new Exception($"Key size of {key.Length} is invalid for mode {mode}");

            this.Key = key;
            this.IV = iv;
            this.Padding = padding.ToString();
            this.Mode = mode.ToString();
        }

        public byte[] Key;
        public byte[] IV;
        public string Padding;
        public string Mode;

        [JsonIgnore]
        public PaddingMode PaddingMode
        {
            get => this.Padding.ToEnum<PaddingMode>();
        }

        [JsonIgnore]
        public CipherMode CipherMode
        {
            get => this.Mode.ToEnum<CipherMode>();
        }
    }

    public static partial class AES
    {
        public static async Task EncryptAsync(this Stream input, AesSecret secret, Stream output)
        {
            using (var aes = new AesManaged())
            {
                aes.Mode = secret.CipherMode;
                aes.Padding = secret.PaddingMode;
                var encryptor = aes.CreateEncryptor(secret.Key, secret.IV);
                var blockSize = aes.LegalBlockSizes.Last();
                
                using (var cryptor = new CryptoStream(output, encryptor, CryptoStreamMode.Write))
                {
                    var read = 0;
                    var bufferSize = blockSize.MaxSize * 1024;
                    var bIn = new byte[bufferSize];
                    while ((read = await input.ReadAsync(bIn, 0, bufferSize)) > 0)
                        await cryptor.WriteAsync(bIn, 0, read);
                }
            }
        }

        public static async Task DecryptAsync(this Stream input, AesSecret secret, Stream output)
        {
            using (var aes = new AesManaged())
            {
                aes.Mode = secret.CipherMode;
                aes.Padding = secret.PaddingMode;
                var encryptor = aes.CreateDecryptor(secret.Key, secret.IV);
                var blockSize = aes.LegalBlockSizes.Last();

                using (var decryptor = new CryptoStream(input, encryptor, CryptoStreamMode.Read))
                {
                    var read = 0;
                    var bufferSize = blockSize.MaxSize * 1024;
                    var bIn = new byte[bufferSize];
                    while ((read = await decryptor.ReadAsync(bIn, 0, bufferSize)) > 0)
                        await output.WriteAsync(bIn, 0, read);
                }
            }
        }
        
        public static async Task EncryptAsync(this FileInfo inputInfo, AesSecret secret, FileInfo outputInfo)
        {
            using (var input = inputInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var output = outputInfo.Open(FileMode.CreateNew, FileAccess.Write, FileShare.None))
                await input.EncryptAsync(secret, output);
        }

        public static async Task DecryptAsync(this FileInfo inputInfo, AesSecret secret, FileInfo outputInfo)
        {
            using (var input = inputInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var output = outputInfo.Open(FileMode.CreateNew, FileAccess.Write, FileShare.None))
                await input.DecryptAsync(secret, output);
        }

        public static AesSecret CreateAesSecret()
            => new AesSecret(RandomEx.NextBytes(32), RandomEx.NextBytes(16));

        public static AesSecret CreateAesSecret(this FileInfo destination)
        {
            var secret = new AesSecret(RandomEx.NextBytes(32), RandomEx.NextBytes(16));
            var jSecret = secret.JsonSerialize();
            FileHelper.WriteAllText(destination.FullName, jSecret);
            return secret;
        }

        public static AesSecret LoadAesSecret(this FileInfo source)
            => FileHelper.ReadAllAsString(source.FullName).JsonDeserialize<AesSecret>();
    }
}
