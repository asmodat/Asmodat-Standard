using System;

namespace AsmodatStandard.Extensions
{
    public static class DecimalEx
    {
        /// <summary>
        /// Adjust size to the value not smaller then increment but also divisible by it without remainder
        /// </summary>
        public static decimal AdjustToPositiveIncrement(this decimal size, decimal increment)
        {
            if (size <= 0)
                return increment;

            var result = (size / increment) * increment;
            size -= result;
            while (size > 0)
            {
                result += increment;
                size -= increment;
            }

            return result;
        }
    }
}
