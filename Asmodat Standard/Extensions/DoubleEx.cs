using System;

namespace AsmodatStandard.Extensions
{
    public static class DoubleEx
    {
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
