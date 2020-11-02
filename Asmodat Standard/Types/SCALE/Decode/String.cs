using AsmodatStandard.Extensions;
using System;
using System.Numerics;

namespace AsmodatStandard.Types
{
    public static partial class Scale
    {
        public static string DecodeString(ref string s)
        {
            if (s.IsNullOrEmpty())
                return null;

            var l = Scale.DecodeCompactInteger(ref s).Value;

            if (l == 0)
                return "";

            if (l < 0 || l > (s.Length/2))
                throw new Exception($"Invalid string length, expected {l} but input has no more than {(s.Length / 2)} characters remaining.");

            var result = Scale.ExtractString(ref s, l);
            return result;
        }

        public static string ExtractString(ref string stringStream, CompactInteger length)
            => ExtractString(ref stringStream, (long)length.Value);
        public static string ExtractString(ref string stringStream, BigInteger length)
            => ExtractString(ref stringStream, (long)length);
        public static string ExtractString(ref string stringStream, long length)
            => ExtractString(ref stringStream, (int)length);
        public static string ExtractString(ref string stringStream, int length)
        {
            string s = string.Empty;
            while (length > 0)
            {
                s += (char)NextByte(ref stringStream);
                length--;
            }
            return s;
        }
    }
}