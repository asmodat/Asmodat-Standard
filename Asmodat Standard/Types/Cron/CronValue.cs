using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AsmodatStandard.Extensions;
using static AsmodatStandard.Types.CronValue;

namespace AsmodatStandard.Types
{
    public class CronValue
    {
        public enum CronUnit
        {
            Minutes = 1,
            Hours = 1 << 1,
            DayOfMonth = 1 << 2,
            Month = 1 << 3,
            DayOfWeek = 1 << 4,
            Year = 1 << 5
        }

        public CronUnit UnitType { get; private set; }
        public CronValue[] InnerValues { get; private set; } = new CronValue[0];

        public bool IsInnerValue { get => (InnerValues?.Length ?? 0) <= 0; }

        /// <summary>
        /// ?
        /// </summary>
        public bool Unspecified { get; private set; } = false;

        /// <summary>
        /// All Values
        /// </summary>
        public bool AllValues { get; private set; } = false;

        public int RangeStart { get; private set; } = -1;

        public int RangeEnd { get; private set; } = -1;

        /// <summary>
        /// Specifies if relates to closest weekday, reange is exact when this value is set
        /// </summary>
        public bool Weekday { get; private set; } = false;

        public int N { get; private set; } = -1;

        public int IncrementShift { get; private set; } = -1;

        public int IncrementModulo { get; private set; } = -1;

        public bool Last { get; private set; } = false;
        public bool First { get; private set; } = false;
        public bool IsEdgeCase { get => Last || First; }
        public bool IsRange { get => RangeStart != RangeEnd; }
        public bool IsExact { get => RangeStart == RangeEnd && RangeStart >= 0 && !Weekday && N == -1; }
        public bool IsModulo { get => IncrementShift >= 0 || IncrementModulo > 0; }

        public CronValue(CronUnit type, bool unspecified, bool allValues, bool last, bool first)
        {
            UnitType = type;
            Unspecified = unspecified;
            AllValues = allValues;
            Last = last;
            First = first;
        }

        public CronValue(CronUnit type, CronValue[] innerValues)
        {
            UnitType = type;
            InnerValues = innerValues;
        }

        public CronValue(CronUnit type, CronValue innerValue)
        {
            UnitType = type;
            InnerValues = new CronValue[1] { innerValue };
        }

        public CronValue(CronUnit type, int incrementShift, int incrementModulo)
        {
            UnitType = type;

            if (incrementShift < 0)
                throw new ArgumentException($"{nameof(incrementShift)} can't be negative.");

            if (incrementModulo <= 0)
                throw new ArgumentException($"{nameof(incrementModulo)} can't be less or equal 0.");

            IncrementShift = incrementShift;
            IncrementModulo = incrementModulo;
        }

        public CronValue(CronUnit type, int rangeStart, int rangeEnd, int n, bool weekday = false)
        {
            if (weekday && type != CronUnit.DayOfMonth)
                throw new ArgumentException($"Weekday can only be set for Unit: '{CronUnit.DayOfMonth}' but was '{type}'.");

            if (rangeStart < 0 || rangeEnd < 0 ||
                (type == CronUnit.Hours && (!rangeStart.InClosedInterval(0, 23) || !rangeEnd.InClosedInterval(0, 23))) ||
                (type == CronUnit.Minutes && (!rangeStart.InClosedInterval(0, 59) || !rangeEnd.InClosedInterval(0, 59))) ||
                (type == CronUnit.DayOfWeek && (!rangeStart.InClosedInterval(1,7) || !rangeEnd.InClosedInterval(1, 7))) ||
                (type == CronUnit.Month && (!rangeStart.InClosedInterval(1, 12) || !rangeEnd.InClosedInterval(1, 12))) ||
                (type == CronUnit.DayOfMonth && (!rangeStart.InClosedInterval(1, 31) || !rangeEnd.InClosedInterval(1, 31))))
                throw new ArgumentException($"Invalid range '{rangeStart}-{rangeEnd}', Unit '{type}'.");

            if (n > 0 && type != CronUnit.DayOfWeek)
                throw new ArgumentException($"Argument nd/nth is only valid for Unit: '{CronUnit.DayOfWeek}' but was '{type}'.");
            else if (type == CronUnit.DayOfWeek && n > 4)
                throw new Exception($"Argument nd/nth its out of allower range <1; 4>., Unit: '{type}'.");

            UnitType = type;
            RangeStart = rangeStart;
            RangeEnd = rangeEnd;
            Weekday = weekday;
            N = n;
        }
    }
}
