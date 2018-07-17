using System;
using System.Security.Cryptography;

namespace AsmodatStandard.Extensions.Cryptography
{
    public static class AesManagedEx
    {
        public static bool IsValidKeySize(this CipherMode mode, int size)
        {
            using (var aes = new AesManaged())
            {
                aes.Mode = mode;
                return aes.ValidKeySize(size);
            }
        }
    }
}
