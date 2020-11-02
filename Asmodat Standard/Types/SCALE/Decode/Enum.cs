using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions;
using System.Linq;

namespace AsmodatStandard.Types
{
    

    public static partial class Scale
    {

        public static object DecodeScaleEnum(ref string str, Type type)
        {
            var genArgType = type.GetGenericArguments().Single(); // enum type
            var e = DecodeEnum(ref str, genArgType); // enum
            var attribute = EnumEx.GetAttributes(e, genArgType).Single() as ScaleEnumTypeAttribute; // enum attribute
            var value = DecodeObject(ref str, attribute.Type);
            var obj = Activator.CreateInstance(type, e, value);
            return obj;
        }

        public static T DecodeEnum<T>(ref string str) where T : Enum
        {
            var arr = EnumEx.ToArray<T>();
            var b = DecodeBytes(ref str, 1)[0];

            for(int i = 0; i < arr.Length; i++)
                if (Convert.ToInt32(arr[i]) == b)
                    return arr[i];

            throw new Exception($"Failed to decode Enum, value of {b} was not found.");
        }

        public static object DecodeEnum(ref string str, Type type)
        {
            var arr = EnumEx.ToArray(type);
            var b = DecodeBytes(ref str, 1)[0];

            for (int i = 0; i < arr.Length; i++)
                if (Convert.ToInt32(arr[i]) == b)
                    return arr[i];

            throw new Exception($"Failed to decode Enum: '{type?.FullName ?? "null"}', value of {b} was not found.");
        }
    }
}