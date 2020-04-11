using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions;


namespace AsmodatStandard.Types
{
    

    public static partial class Scale
    {
        public static byte[] DecodeBytes(ref string str)
        {
            if (str == null)
                return null;

            var count = Scale.DecodeCompactInteger(ref str);
            return DecodeBytes(ref str, (long)count.Value);
        }

        public static byte[] DecodeBytes(ref string str, long count)
        {
            if(str.Length * 2 < count)
                throw new Exception($"Can't decode bytes from string, expected {count}, got {str.Length/2} characters");

            var arr = GetBytesFromHexString(str, count);
            str = str.Substring(arr.Length * 2);
            return arr;
        }

        public static byte[] GetBytesFromHexString(string str, long count = int.MaxValue)
        {
            if (str == null)
                return null;

            if (str.Length == 0 || count == 0)
                return new byte[0];

            if (str.Length % 2 != 0)
                throw new Exception("String is not a hex byte string");

            var bytes = new List<byte>();
            for (int i = 0; i < str.Length;)
            {
                bytes.Add(byte.Parse(str.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
                i += 2;

                if (bytes.Count >= count)
                    break;

            }
            return bytes.ToArray();
        }
    }
}