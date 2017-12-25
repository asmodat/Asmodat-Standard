using System;

namespace AsmodatStandard.Extensions
{
    public static class DateTimeHelper
    {
        public static DateTime CoallesceDefault(this DateTime dt, DateTime value) => dt.Ticks == default(DateTime).Ticks ? value : dt;

        /// <summary>
        /// Converts unix 'timestamp' into UTC DateTime
        /// </summary>
        public static DateTime ToDateTimeFromUnixTimeStap(this double timestamp) => new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();

        public static DateTime ToLocalDateTimeFromUnixTimeStap(this double timestamp) => timestamp.ToDateTimeFromUnixTimeStap().ToLocalTime();
    }
}
