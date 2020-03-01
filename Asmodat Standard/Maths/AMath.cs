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

        public static decimal GetMedian(decimal[] sourceNumbers)
        {  
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                throw new Exception("Median of empty array not defined.");

           var sortedPNumbers = (decimal[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);

            int size = sortedPNumbers.Length;
            int mid = size / 2;
            decimal median = (size % 2 != 0) ? (decimal)sortedPNumbers[mid] : ((decimal)sortedPNumbers[mid] + (decimal)sortedPNumbers[mid - 1]) / 2;
            return median;
        }

        public static double GetMedian(double[] sourceNumbers)
        {
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                throw new Exception("Median of empty array not defined.");

            var sortedPNumbers = (double[])sourceNumbers.Clone();
            Array.Sort(sortedPNumbers);

            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }
    }
}