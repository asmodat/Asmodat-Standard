using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class IntEx
    {
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
    }
}
