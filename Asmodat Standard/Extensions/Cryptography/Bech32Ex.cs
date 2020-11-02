using AsmodatStandard.Cryptography.Bitcoin;

namespace AsmodatStandard.Extensions.Cryptography
{
    public static class Bech32Ex
    {
        /// <summary>
        /// Changes prefix of the encoded bech32 string
        /// </summary>
        public static string ReEncode(string hrp, string encoded) => Bech32.Encode(hrp, DecodeBytes(encoded));
        public static byte[] DecodeBytes(string encoded) => Bech32.Decode(encoded, out var thrp);
        
        public static bool TryDecode(string encoded, out string hrp, out byte[] bytes)
        {
            try
            {
                bytes = Bech32.Decode(encoded, out var thrp);
                hrp = thrp?.Trim(' ','\n','\r','\t');
                return true;
            }
            catch
            {
                hrp = null;
                bytes = null;
                return false;
            }
        }

        public static bool CanDecode(string encoded)
        {
            try
            {
                var bytes = Bech32.Decode(encoded, out var thrp);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

