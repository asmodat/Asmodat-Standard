﻿using System;
using System.Globalization;

namespace AsmodatStandard.Extensions
{
    public static class DateTimeEx
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static long UnixTimestampNow() => (long)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
        public static DateTime ToDateTimeFromUnixTimestamp(this long seconds) => UnixEpoch.AddSeconds(seconds);

        public static string ToRfc3339String(this DateTime dt)
            => dt.ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz", DateTimeFormatInfo.InvariantInfo);
        
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            if (timeSpan == TimeSpan.Zero)
                return dateTime;

            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }

        public static bool IsUTC(this DateTime dt)
            => dt.Kind == DateTimeKind.Utc;

        public static string ToLongDateTimeString(this DateTime dt, string separator = " ")
            => $"{dt.ToLongDateString()}{separator}{dt.ToLongTimeString()}";

        public static string ToShortDateTimeString(this DateTime dt, string separator = " ")
            => $"{dt.ToShortDateString()}{separator}{dt.ToShortTimeString()}";

        public static DateTime NthDayOfMonth(this DateTime dt, int nth, DayOfWeek dayOfWeek)
        {
            if (nth < 1 || nth > 4)
                throw new ArgumentException($"{nameof(nth)} was out of range <1; 4>");

            var start = new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind);

            for(int i = 0; i < nth; i++)
                while (start.DayOfWeek != dayOfWeek)
                    start.AddDays(1);

            return start;
        }
    }
}
