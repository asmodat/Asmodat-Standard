using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class ObjectEx
    {
        public static Dictionary<string, T> JsonConvertToDictionary<T>(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            return dictionary;
        }

        public static bool IsNotNull(this object value)
        {
            return value != null;
        }

        public static T JsonCopy<T>(this T obj) => obj.JsonSerialize(Formatting.None).JsonDeserialize<T>();
        public static T JsonCast<T>(this object obj) => obj.JsonSerialize(Formatting.None).JsonDeserialize<T>();

        public static string JsonSerialize(this object obj, Formatting formatting = Formatting.None,
            ReferenceLoopHandling referenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling nullValueHandling = NullValueHandling.Include)
            => JsonConvert.SerializeObject(obj, formatting,
                new JsonSerializerSettings {
                    ReferenceLoopHandling = referenceLoopHandling,
                    NullValueHandling = nullValueHandling
                });

        public static bool JsonEquals(this object o1, object o2) 
            => o1.JsonSerialize() == o2.JsonSerialize();

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
        public static uint ToUInt32(this object obj) => Convert.ToUInt32(obj);
        public static long ToInt64(this object obj) => Convert.ToInt64(obj);
        public static ulong ToUInt64(this object obj) => Convert.ToUInt64(obj);

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
