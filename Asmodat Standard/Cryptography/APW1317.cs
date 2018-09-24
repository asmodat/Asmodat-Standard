using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Cryptography
{
    /// <summary>
    /// Asmodat Password Wrapper 13/17
    /// </summary>
    public static class APW1317
    {
        private static int origin = 1642197419;

        public static string Generate(string password, int difficulty)
        {
            if (difficulty > (int.MaxValue - origin) || difficulty <= 0)
                throw new ArgumentException($"Dyfficulty {difficulty} must be in range of [1, {int.MaxValue - origin}]");

            var hash = password.SHA256();
            var noise = RandomEx.Next(origin, origin + difficulty).ToByteArray();
            return hash.Merge(noise).SHA256().ToHexString();
        }

        public static bool Verify(string password, string secret, int difficulty)
        {
            if (difficulty > (int.MaxValue - origin) || difficulty <= 0)
                throw new ArgumentException($"Dyfficulty {difficulty} must be in range of [1, {int.MaxValue - origin}]");

            var hash = password.SHA256();

            for (int i = origin; i <= origin + difficulty; i++)
                if (hash.Merge(i.ToByteArray()).SHA256().ToHexString() == secret)
                    return true;

            return false;
        }
    }
}
