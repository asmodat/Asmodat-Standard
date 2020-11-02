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
        public static object DecodeScaleOption(ref string str, Type type)
        {
            var hasValue = HasOption(ref str);
            var genArgType = type.GetGenericArguments().Single();
            var value = hasValue ?
                DecodeObject(ref str, genArgType) :
                genArgType.GetDefaultObject();

            return Activator.CreateInstance(type, hasValue, value);
        }

        public static bool HasOption(ref string str)
        {
            if (str == null)
                return false;

            var hasOption = DecodeBytes(ref str, 1)[0];

            return hasOption == 1;
        }

        public static bool? DecodeOptionBool(ref string str)
        {
            var b = DecodeBytes(ref str, 1)[0];
            if (b == 0)
                return null;
            else if (b == 1)
                return false;
            else if (b == 2)
                return true;

            throw new Exception($"Scale.DecodeOptionBool => Invalid boolean or option value: {b}, expected one of [0,1,2]");
        }
    }
}