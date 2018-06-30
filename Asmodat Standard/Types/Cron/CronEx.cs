using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AsmodatStandard.Extensions;
using Newtonsoft.Json;
using static AsmodatStandard.Types.CronValue;

namespace AsmodatStandard.Types
{
    public static class CronEx
    {
        public static Cron ToCron(this string cron)
        {
            if (cron.IsNullOrEmpty())
                throw new ArgumentNullException($"{nameof(cron)}");

            var split = cron.Trim().Split(' ');

            if (split?.Length != 6 && split?.Any(cv => cv.IsNullOrWhitespace()) != false)
                throw new ArgumentException($"Expected 6 cron non null or empty value expressions but got {split?.Length}, Cron expression: '{cron}'");

            return new Cron(
                CronValueEx.ToCronValue(split[0], CronUnit.Minutes),
                CronValueEx.ToCronValue(split[1], CronUnit.Hours),
                CronValueEx.ToCronValue(split[2], CronUnit.DayOfMonth),
                CronValueEx.ToCronValue(split[3], CronUnit.Month),
                CronValueEx.ToCronValue(split[4], CronUnit.DayOfWeek),
                CronValueEx.ToCronValue(split[5], CronUnit.Year),
                origin: cron);
        }

        public static int Compare(this Cron c, DateTime dt)
        {
            if (!dt.IsUTC())
                throw new ArgumentException($"{nameof(dt)} must be UTC, but was {dt.Kind}");

            var m = c.Minutes.CompareBase(dt.Minute);
            var H = c.Hours.CompareBase(dt.Hour);
            int d;
            
            if(c.DayOfWeek.N > 0)
            {
                var day = dt.NthDayOfMonth(c.DayOfWeek.N, (DayOfWeek)(c.DayOfWeek.RangeStart + 1));
                if (day.Day == dt.Day)
                    d = 0;
                else if (day.Day > dt.Day)
                    d = 1;
                else
                    d = -1;
            }
            else
                d = c.DayOfWeek.CompareBase((int)dt.DayOfWeek + 1);

            var M = c.Month.CompareBase(dt.Month);
            int D = -2;

            if(c.DayOfMonth.Weekday)
            {
                var dtNew = new DateTime(dt.Year, dt.Month, c.DayOfMonth.RangeStart, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc);

                if (dt.DayOfWeek == DayOfWeek.Saturday)
                {
                    if (dtNew.Day <= 1)
                        D = -1;

                    dtNew.AddDays(-1);
                }
                else if (dt.DayOfWeek == DayOfWeek.Sunday)
                    dtNew.AddDays(1);

                if (D == -2) //not set
                {
                    if (dt.Day == dtNew.Day)
                        D = 0;
                    else if (dt.Day < dtNew.Day)
                        D = 1;
                    else
                        D = -1;
                }
            }
            else
                D = c.DayOfMonth.CompareBase(dt.Day);

            var y = c.Year.CompareBase(dt.Year);

            if (y == -1) return -1;
            else if(y == 1) return 1;

            if (M == -1) return -1;
            else if (M == 1) return 1;

            if (D == -1) return -1;
            else if (D == 1) return 1;

            if (d == -1) return -1;
            else if (d == 1) return 1;

            if (H == -1) return -1;
            else if (H == 1) return 1;

            if (m == -1) return -1;
            else if (m == 1) return 1;

            if (y == 0 && M == 0 && D == 0 && d == 0 && H == 0 && m == 0)
                return 0;

            throw new NotSupportedException($"Unsuported state found during comparison with Date: {dt.ToLongDateTimeString()}, Cron: {c.JsonSerialize()}");
        }

        public static int CompareBase(this CronValue cv, int val)
        {
            if (cv.AllValues)
                return 0;

            int min = cv.UnitType.Min(), max = cv.UnitType.Max();

            if (cv.IsEdgeCase)
            {
                if(cv.First)
                {
                    if (val == min)
                        return 0;
                    else if (val > min)
                        return -1;
                }

                if (cv.Last)
                {
                    if (val == max)
                        return 0;
                    else if (val < max)
                        return 1;  
                }

                throw new NotSupportedException($"#1, {nameof(CompareBase)}, Cron: {cv.JsonSerialize(Formatting.Indented)}, Val: {val}");
            }

            if(cv.IsRange)
            {
                if (cv.RangeStart <= cv.RangeEnd)
                {
                    if (val >= cv.RangeStart && val <= cv.RangeEnd)
                        return 0;

                    if (val < cv.RangeStart)
                    {
                        var dR = max - cv.RangeEnd;
                        var dL = cv.RangeStart - min;
                        if (val + dR >= dL - val)
                            return 1;
                        else
                            return -1;
                    }
                    else if (val > cv.RangeEnd)
                    {
                        var dL = cv.RangeStart - min;

                        if (val - cv.RangeEnd <= ((max - val) + dL))
                            return -1;
                        else
                            return 1;
                    }
                }
                else if (cv.RangeStart >= cv.RangeEnd)
                {
                    if (val >= cv.RangeStart || val <= cv.RangeEnd)
                        return 0;

                    if (cv.RangeStart - val >= val - cv.RangeEnd)
                        return 1;
                    else
                        return -1;
                }

                throw new NotSupportedException($"#2, {nameof(CompareBase)}, Cron: {cv.JsonSerialize(Formatting.Indented)}, Val: {val}");
            }

            if(cv.InnerValues.Length > 0)
            {
                if (cv.InnerValues.Any(x => x.CompareBase(val) == 0))
                    return 0;

#warning Missing Unit Tests

                if (cv.InnerValues.Any(x => x.CompareBase(val) == 1))
                    return 1;

                if (cv.InnerValues.Any(x => x.CompareBase(val) == -1))
                    return -1;

                throw new NotSupportedException($"#3, {nameof(CompareBase)}, Cron: {cv.JsonSerialize(Formatting.Indented)}, Val: {val}");
            }

            if(cv.IsModulo)
            {
                if ((val - cv.IncrementShift) % cv.IncrementModulo == 0)
                    return 0;

#warning Missing Unit Tests

                if ((val - cv.IncrementShift) + cv.IncrementModulo > max)
                    return -1;

                if ((val - cv.IncrementShift) + cv.IncrementModulo <= max)
                    return 1;

                throw new NotSupportedException($"#4, {nameof(CompareBase)}, Cron: {cv.JsonSerialize(Formatting.Indented)}, Val: {val}");
            }

            if(cv.IsExact)
            {
                if (val < cv.RangeStart)
                    return 1;
                else if (val == cv.RangeStart)
                    return 0;
                else
                    return -1;
            }

            throw new NotSupportedException($"#5, {nameof(CompareBase)}, Cron: {cv.JsonSerialize(Formatting.Indented)}, Val: {val}");
        }
    }
}
