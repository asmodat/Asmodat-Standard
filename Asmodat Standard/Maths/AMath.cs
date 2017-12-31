using System;
using AsmodatStandard.Extensions;

namespace AsmodatStandard.Maths
{
    public partial class AMath
    {
        /// <summary>
        /// Returns value from the range of [Min, Max]
        /// </summary>
        public static double MinMax(double min, double value, double max)
        {
            if (min > max || max < min)
                throw new ArgumentException("min must be <=  max, and max >= min");

            if(min.IsNaN() || value.IsNaN() || max.IsNaN())
                throw new ArgumentException("min, max nor value cannot be NaN");

            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }
    }
}