using System;

namespace AsmodatStandard.Types
{
    public static partial class Scale
    {
        public static bool DecodeBool(ref string str)
        {
            var b = DecodeBytes(ref str, 1)[0];
            if (b == 0)
                return false;
            else if (b == 1)
                return true;

            throw new Exception($"Scale.DecodeBool => Invalid boolean value: {b}, make sure it is not a boolean option, refer to https://substrate.dev/docs/en/conceptual/core/codec");
        }
    }
}