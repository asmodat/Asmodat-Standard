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
        public static UInt16[] DecodeVecUInt16(string str)
        {
            if (str == null)
                return null;

            var l = Scale.DecodeCompactInteger(ref str);

            if (l.Value == 0)
                return new UInt16[0];

            var bytes = GetBytesFromHexString(str);
            var size = (int)(bytes.Length / l.Value); //number of bytes per item
            var result = new List<UInt16>();
            for(int i = 0; i < bytes.Length;)
            {
                var value = bytes.SubArray(i, size);
                result.Add(DecodeUInt16(value));
                i += size;
            }

            return result.ToArray();
        }
    }
}