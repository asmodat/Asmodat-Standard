using AsmodatStandard.Extensions.Collections;
using System;
using System.Linq;

namespace AsmodatStandard.Maths
{
    public partial class AMath
    {
        /// <summary>
        /// confidence must be from (0 to 100)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="average"></param>
        /// <param name="confidence"></param>
        /// <param name="population"></param>
        /// <returns></returns>
        public static double StandarConfidence(double[] data, double average, double confidence, bool population = false)
        {
            if (data == null || data.Length <= 0) return double.NaN;
            double sigma = Math.Sqrt(Variance(data, average, population));
            double multiplayer = ((AExcel.NORMSINV((double)confidence / 100) + 1.5) / 2);
            return sigma * multiplayer;
        }


        /// <summary>
        /// Standard deviation σ (sigma) is a measure of how spread out numbers are, it is a squer root of the variance
        /// set population to true then return sqrt(sum / (double)length); -> for standard deviation Variance
        /// set population to false then return sqrt(sum / (double)(length - 1)); -> population standard deviation variance
        /// </summary>
        public static void StandarDeviation(double[] data, out double avg, out double sigma, bool population = false)
        {
            avg = data.Average();
            sigma = StandarDeviation(data, avg, population);
        }


        /// <summary>
        /// Standard deviation σ (sigma) is a measure of how spread out numbers are, it is a squer root of the variance
        /// set population to true then return sqrt(sum / (double)length); -> for standard deviation Variance
        /// set population to false then return sqrt(sum / (double)(length - 1)); -> population standard deviation variance
        /// </summary>
        /// <param name="data"></param>
        /// <param name="population"></param>
        /// <returns></returns>
        public static double StandarDeviation(double[] data, bool population = false)
            => StandarDeviation(data, data.Average(), population);

        /// <summary>
        /// Standard deviation σ (sigma) is a measure of how spread out numbers are
        /// it is a squer root of the variance
        /// 
        /// set population to true then return sqrt(sum / (double)length); -> for standard deviation Variance
        /// set population to false then return sqrt(sum / (double)(length - 1)); -> population standard deviation variance
        /// </summary>
        /// <param name="data"></param>
        /// <param name="average"></param>
        /// <param name="population"></param>
        /// <returns></returns>
        public static double StandarDeviation(double[] data, double average, bool population = false)
            => data.IsNullOrEmpty() ? double.NaN : Math.Sqrt(Variance(data, average, population));

        /// <summary>
        /// Standard deviation σ (sigma) is a measure of how spread out numbers are
        /// it is a squer root of the variance
        /// </summary>
        /// <param name="variance"></param>
        /// <returns></returns>
        public static double StandarDeviation(double variance) => Math.Sqrt(variance);
    }
}
