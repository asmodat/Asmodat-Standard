using System.Numerics;

namespace AsmodatStandard.Types
{
    public class SI
    {
        public const string yotta = "000000000000000000000000";
        public const string zetta = "000000000000000000000";
        public const string exa =   "000000000000000000";
        public const string peta =  "000000000000000";
        public const string tera =  "000000000000";
        public const string giga =  "000000000";
        public const string mega =  "000000";
        public const string kilo =  "000";
        public const string hecto = "00";
        public const string deca =  "0";

        public static BigInteger Y = BigInteger.Parse($"1{SI.yotta}");
        public static BigInteger Z = BigInteger.Parse($"1{SI.zetta}");
        public static BigInteger E = BigInteger.Parse($"1{SI.exa}");
        public static BigInteger P = BigInteger.Parse($"1{SI.peta}");
        public static BigInteger T = BigInteger.Parse($"1{SI.tera}");
        public static BigInteger G = BigInteger.Parse($"1{SI.giga}");
        public static BigInteger M = BigInteger.Parse($"1{SI.mega}");
        public static BigInteger k = BigInteger.Parse($"1{SI.kilo}");
        public static BigInteger h = BigInteger.Parse($"1{SI.hecto}");
        public static BigInteger da = BigInteger.Parse($"1{SI.deca}");
    }
}
