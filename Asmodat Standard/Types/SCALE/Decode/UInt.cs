using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions;
using System.Globalization;

namespace AsmodatStandard.Types
{
    

    public static partial class Scale
    {
        public static UInt16 DecodeUInt16(ref string stringStream) => DecodeUInt16(DecodeBytes(ref stringStream, 2));

        public static UInt32 DecodeUInt32(ref string stringStream) => DecodeUInt32(DecodeBytes(ref stringStream, 4));

        public static UInt64 DecodeUInt64(ref string stringStream) => DecodeUInt64(DecodeBytes(ref stringStream, 8));

        public static BigInteger DecodeUInt128(ref string stringStream) 
            => new BigInteger(DecodeBytes(ref stringStream, 16).Merge(new byte[] { 0 })); //last byte must be 0 to ensure value is positive
        

        public static UInt16 DecodeUInt16(byte[] arr)
        {
            if (arr?.Length != 2)
                throw new Exception("Array is not an UInt16 value");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(arr);

            return BitConverter.ToUInt16(arr, 0);
        }

        public static UInt32 DecodeUInt32(byte[] arr)
        {
            if (arr?.Length != 4)
                throw new Exception("Array is not an UInt32 value");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(arr);

            return BitConverter.ToUInt32(arr, 0);
        }

        public static UInt64 DecodeUInt64(byte[] arr)
        {
            if (arr?.Length != 8)
                throw new Exception("Array is not an UInt64 value");

            if (!BitConverter.IsLittleEndian)
                Array.Reverse(arr);

            return BitConverter.ToUInt64(arr, 0);
        }
    }
}