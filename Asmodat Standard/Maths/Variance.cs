using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmodatStandard.Maths
{
    public partial class AMath
    {
        /// <summary>
        /// The averange of the squared differences from the mean, in other words it is a measurment of how far a set of numbers is spread out.
        /// Zero indicates, that all values are identical, this value is always positive
        /// 
        /// set population to true then return sum / (double)length; -> for standard deviation Variance
        /// set population to false then return sum / (double)(length - 1) -> population standard deviation variance
        /// </summary>
        /// <param name="data"></param>
        /// <param name="population"></param>
        /// <returns></returns>
        public static double Variance(double[] data, bool population = false)
        {
            if (data == null || data.Length <= 0) return double.NaN;
            return AMath.Variance(data, data.Average(), population);
        }

        /// <summary>
        /// The averange of the squared differences from the mean
        /// 
        /// set population to true then return sum / (double)length; -> for standard deviation Variance
        /// set population to false then return sum / (double)(length - 1) -> population standard deviation variance
        /// </summary>
        /// <param name="data"></param>
        /// <param name="average"></param>
        /// <param name="population"></param>
        /// <returns></returns>
        public static double Variance(double[] data, double average, bool population = false)
        {
            double sum = 0.0;
            for (int i = 0; i < data.Length; i++)
                sum += (data[i] - average) * (data[i] - average); //faster then math pow

            return sum / (population ? data.Length : (data.Length - 1));
        }


        public static double Variance(double[] data, double average, double last, bool population = false)
        {
            double variance;

            if (double.IsNaN(last))
                variance = AMath.Variance(data, average, population);
            else
            {
                int LN1 = data.Length - 1;
                double sum = last * (population ? LN1 + 1 : LN1);
                sum += (data[LN1] - average) * (data[LN1] - average);
                variance = sum / (population ? LN1 + 1 : LN1);
            }

            return variance;
        }
    }
}
