using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AsmodatStandard.Types;

namespace AsmodatStandard.Extensions
{
    public static class BigIntEx
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Metric_prefix
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToSI1000String(this BigInteger b)
        {
            var v = b.ToString();
            if (v.EndsWith(SI.yotta))
                v = $"{v.TrimEndSingle(SI.yotta)}Y";
            else if (v.EndsWith(SI.zetta))
                v = $"{v.TrimEndSingle(SI.zetta)}Z";
            else if (v.EndsWith(SI.exa))
                v = $"{v.TrimEndSingle(SI.exa)}E";
            else if (v.EndsWith(SI.peta))
                v = $"{v.TrimEndSingle(SI.peta)}P";
            else if (v.EndsWith(SI.tera))
                v = $"{v.TrimEndSingle(SI.tera)}T";
            else if (v.EndsWith(SI.giga))
                v = $"{v.TrimEndSingle(SI.giga)}G";
            else if (v.EndsWith(SI.mega))
                v = $"{v.TrimEndSingle(SI.mega)}M";
            else if (v.EndsWith(SI.kilo))
                v = $"{v.TrimEndSingle(SI.kilo)}k";

            return v;
        }

        public static string TrySIDecode(string s)
        {
            if (s.IsNullOrWhitespace())
                return s;

            s = s.Replace("da", SI.deca);
            s = s.Replace("h", SI.hecto);
            s = s.Replace("k", SI.kilo);
            s = s.Replace("M", SI.mega);
            s = s.Replace("G", SI.giga);
            s = s.Replace("T", SI.tera);
            s = s.Replace("P", SI.peta);
            s = s.Replace("E", SI.exa);
            s = s.Replace("Z", SI.zetta);
            s = s.Replace("Y", SI.yotta);

            return s;
        }

        public static BigInteger ToBigIntFromSI(this string s) => BigInteger.Parse(BigIntEx.TrySIDecode(s));

        public static BigInteger ToBigIntOrDefaultFromSI(this string s, BigInteger @default = default(BigInteger))
        {
            if (s.IsNullOrWhitespace())
                return @default;

            s = BigIntEx.TrySIDecode(s);

            if (BigInteger.TryParse(s, out var result))
                return result;
            else
                return @default;
        }
    }
}
