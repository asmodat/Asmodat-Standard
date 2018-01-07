using System;

namespace AsmodatStandard.Extensions
{
    public static class DoubleEx
    {
        public static string ToString(this double d, int nDecimals)
            => d.ToString($"N{nDecimals}");

        public static bool IsNaN(this double d) => double.IsNaN(d);
        public static bool IsZeroOrNaN(this double d) => double.IsNaN(d) || d == 0;
        public static bool IsZeroOrNaN(this double? d) => d == null || d.Value.IsZeroOrNaN();

        public static double CoalesceValue(this double d, double value, double toReplace) => 
            ((double.IsNegativeInfinity(value) && double.IsNegativeInfinity(value)) ||
            (double.IsPositiveInfinity(value) && double.IsPositiveInfinity(value)) ||
            (double.IsNaN(value) && double.IsNaN(value))) || d == value ? value : toReplace;

        public static double CoalesceZero(this double d, double value) => d == 0 ? value : d;
        public static double CoalesceZeroOrNaN(this double d, double value) => double.IsNaN(d) || d == 0 ? value : d;
        public static double CoalesceNaN(this double d, double value) => double.IsNaN(d) ? value : d;
        public static double CoalesceNaN(this double? d, double value) => (d == null || double.IsNaN(d.Value)) ? value : d.Value;

        /// <summary>
        /// This method cuts value afrer specified number of decimals, no rounding is done
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static double Truncate(this double value, uint decimals)
        {
            double pow = Math.Pow(10, decimals);
            return Math.Truncate(pow * value) / pow;
        }

        public static double Average(this double[] input)
        {
            double sum = input[0];
            for (int i = 1; i < input.Length; i++)
                sum += input[i];

            return sum / input.Length;
        }

        public static double Max(this double[] input)
        {
            double output = input[0];
            for (int i = 1; i < input.Length; i++)
                if (input[i] > output)
                    output = input[i];

            return output;
        }

        public static double Min(this double[] input)
        {
            double output = input[0];
            for (int i = 1; i < input.Length; i++)
                if (input[i] < output)
                    output = input[i];

            return output;
        }

        /// <summary>
        /// Copares two doubles with specified precision
        /// Example:
        /// precision = 0.1
        /// 0.9 == 1 -> true
        /// 0.8 == 1 -> false
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="precision"></param>
        /// <returns>returns true if |v1 - v2| is les or equal to precision, else false </returns>
        public static bool Equals(double v1, double v2, double precision) => (Math.Abs(v1 - v2) <= precision);
    }
}
