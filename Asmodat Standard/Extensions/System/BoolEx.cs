using System;

namespace AsmodatStandard.Extensions
{
    public static class BoolEx
    {
        public static bool IsTrue(this bool? b) => b != null && b.Value ? true : false;
        public static bool IsFalse(this bool? b) => b != null && !b.Value ? true : false;

        /// <summary>
        /// true == 1 | false == 0
        /// </summary>
        public static int ToInt(this bool b) => b ? 1 : 0;

        /// <summary>
        /// true == 1 | false == 0 or null
        /// </summary>
        public static int ToInt(this bool? b) => b != null && b.Value ? 1 : 0;

        /// <summary>
        /// true == 1 else false
        /// </summary>
        public static int ToBool(this Int32 v) => v == 1 ? 1 : 0;
    }
}
