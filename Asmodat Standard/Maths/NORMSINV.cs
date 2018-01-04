using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsmodatStandard.Maths
{
    public partial class AExcel
    {
        /// <summary>
        /// Coefitients in normal approximations
        /// </summary>
        static double[] NORMa = new double[6]{-3.969683028665376e+01,  2.209460984245205e+02,
                      -2.759285104469687e+02,  1.383577518672690e+02,
                      -3.066479806614716e+01,  2.506628277459239e+00};

        /// <summary>
        /// Coefitients in normal approximations
        /// </summary>
        static double[] NORMb = new double[5]{-5.447609879822406e+01,  1.615858368580409e+02,
                      -1.556989798598866e+02,  6.680131188771972e+01,
                      -1.328068155288572e+01 };

        /// <summary>
        /// Coefitients in normal approximations
        /// </summary>
        static double[] NORMc = new double[6]{-7.784894002430293e-03, -3.223964580411365e-01,
                      -2.400758277161838e+00, -2.549732539343734e+00,
                      4.374664141464968e+00,  2.938163982698783e+00};

        /// <summary>
        /// Coefitients in normal approximations
        /// </summary>
        static double[] NORMd = new double[4]{7.784695709041462e-03, 3.224671290700398e-01,
                       2.445134137142996e+00,  3.754408661907416e+00};

        /// <summary>
        /// Norm low approximation break point
        /// </summary>
        const double NORMplow = 0.02425;

        /// <summary>
        /// Norm high approximation break point
        /// </summary>
        const double NORMphigh = 0.97575;

        /// <summary>
        /// Returns the inverse of the standard normal cumulative distribution. The distribution has a mean of zero and a standard deviation of one.
        /// 
        /// Convertion from confidence in % (from (0 to 100)) to sigma can be calculated like this: (AExcel.NORMSINV((double)confidence / 100) + 1.5) / 2;
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double NORMSINV(double p)
        {
            if(p <=0 || p >= 1) return double.NaN;
            double q;

            //Rational approximation for lower region
            if (p < NORMplow)
            {
                q = Math.Sqrt(-2 * Math.Log(p));
                return (((((NORMc[0] * q + NORMc[1]) * q + NORMc[2]) * q + NORMc[3]) * q + NORMc[4]) * q + NORMc[5]) /
                                             ((((NORMd[0] * q + NORMd[1]) * q + NORMd[2]) * q + NORMd[3]) * q + 1);
            }

            //Rational approximation for upper region
            if (NORMphigh < p)
            {
                q = Math.Sqrt(-2 * Math.Log(1 - p));
                return -(((((NORMc[0] * q + NORMc[1]) * q + NORMc[2]) * q + NORMc[3]) * q + NORMc[4]) * q + NORMc[5]) /
                                                    ((((NORMd[0] * q + NORMd[1]) * q + NORMd[2]) * q + NORMd[3]) * q + 1);
            }

            //rational approximation for central region
            double q1 = p - 0.5;
            var r = q1 * q1;

            return (((((NORMa[0] * r + NORMa[1]) * r + NORMa[2]) * r + NORMa[3]) * r + NORMa[4]) * r + NORMa[5]) * q1 /
                             (((((NORMb[0] * r + NORMb[1]) * r + NORMb[2]) * r + NORMb[3]) * r + NORMb[4]) * r + 1);
        }

    }
}
