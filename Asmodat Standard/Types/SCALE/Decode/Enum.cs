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
        public static T DecodeEnum<T>(ref string str) where T : Enum
        {
            var arr = EnumEx.ToArray<T>();
            var b = DecodeBytes(ref str, 1)[0];

            for(int i = 0; i < arr.Length; i++)
                if (Convert.ToInt32(arr[i]) == b)
                    return arr[i];

            throw new Exception($"Failed to decode Enum, value of {b} was not found.");
        }
    }
}