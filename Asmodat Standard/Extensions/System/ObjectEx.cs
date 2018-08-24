using Newtonsoft.Json;
using System;

namespace AsmodatStandard.Extensions
{
    public static class ObjectEx
    {
        public static string JsonSerialize(this object obj, Formatting formatting = Formatting.None,
            ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling nullValueHandling = NullValueHandling.Include)
            => JsonConvert.SerializeObject(obj, formatting,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = referenceLoopHandling,
                    NullValueHandling = nullValueHandling
                });

        public static double ToDoubleOrNaN(this object obj)
        {
            if (obj == null)
                return double.NaN;

            try
            {
                return Convert.ToDouble(obj);
            }
            catch
            {
                return double.NaN;
            }
        }

        public static double ToDouble(this object obj) => obj == null ? double.NaN : Convert.ToDouble(obj);
        public static int ToInt32(this object obj) => Convert.ToInt32(obj);
        public static long ToInt64(this object obj) => Convert.ToInt64(obj);

        public static string[] ToStringArray(this object[] input)
        {
            if (input == null)
                return null;

            var output = new string[input.Length];
            return Array.ConvertAll(input, x => x.ToString());
        }

        public static DateTime[] ToDateTimeArrayFromOADate(this object[] input)
        {
            if (input == null)
                return null;

            var output = new DateTime[input.Length];
            return Array.ConvertAll(input, x => DateTime.FromOADate(x.ToDouble()));
        }

        /// <summary>
        /// Converts all null object to NaN and others to double
        /// </summary>
        public static double[] ToDoubleOrNaNArray(this object[] input)
        {
            if (input == null)
                return null;

            var output = new double[input.Length];
            return Array.ConvertAll(input, x => x.ToDoubleOrNaN());
        }

        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
