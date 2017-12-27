using System;

namespace AsmodatStandard.Extensions
{
    public static class BoolEx
    {
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
