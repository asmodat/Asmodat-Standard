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