using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class IntEx
    {
#warning IsPowerOf2 method has no unit tests
        public static bool IsPowerOf2(this int v)
        {
            if (v < 0)
                throw new Exception("Value can't be negative");

            return v != 0 && (v & (v - 1)) == 0;
        }

        public static int[] GetBitShiftPositions(this int i)
        {
            if (i == 0)
                return new int[0];

            var positions = new List<int>();

            for(int s = 0; s < 32; s++)
                if (((1 << s) & i) != 0)
                    positions.Add(s);

            return positions.ToArray();
        }

        public static byte[] ToByteArray(this int i) => BitConverter.GetBytes(i);
        
    }
}
