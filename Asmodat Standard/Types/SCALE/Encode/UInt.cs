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
        public static string EncodeUInt16(UInt16 v)
        {
            var arr = BitConverter.GetBytes(v);

            if (arr?.Length != 2)
                throw new Exception("Array is not an UInt16 value");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(arr);

            return arr.ToHexString();
        }

        public static string EncodeUInt32(UInt32 v)
        {
            var arr = BitConverter.GetBytes(v);

            if (arr?.Length != 4)
                throw new Exception("Array is not an UInt16 value");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(arr);

            return arr.ToHexString();
        }

        public static string EncodeUInt64(UInt64 v)
        {
            var arr = BitConverter.GetBytes(v);

            if (arr?.Length != 8)
                throw new Exception("Array is not an UInt16 value");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(arr);

            return arr.ToHexString();
        }
    }
}