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
        public static byte[][] DecodeVecFixedBytes(string str)
        {
            if (str == null)
                return null;

            var l = DecodeCompactInteger(ref str);

            if (l.Value == 0)
                return new byte[0][];

            var bytes = GetBytesFromHexString(str);
            var bytesPeritem = (int)(bytes.Length / l.Value); //number of bytes per item
            var result = new List<byte[]>();
            for (int i = 0; i < bytes.Length;)
            {
                result.Add(DecodeBytes(ref str, bytesPeritem));
                i += bytesPeritem;
            }

            if (!str.IsNullOrEmpty())
                throw new Exception("DecodeVecFixedBytes => faulre, string was not a Vec.");

            return result.ToArray();
        }

        public static byte[][] DecodeVecFixedBytes(ref string str, int bytesPeritem)
        {
            if (str == null)
                return null;

            var l = DecodeCompactInteger(ref str);

            if (l.Value == 0)
                return new byte[0][];

            var bytes = GetBytesFromHexString(str);
            var result = new List<byte[]>();
            for (int i = 0; i < bytes.Length;)
            {
                result.Add(DecodeBytes(ref str, bytesPeritem));
                i += bytesPeritem;
            }

            return result.ToArray();
        }
    }
}