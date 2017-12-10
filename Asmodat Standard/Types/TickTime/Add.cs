using System;

namespace AsmodatStandard.Types
{
    public partial struct TickTime : IComparable, IEquatable<TickTime>, IEquatable<long>, IComparable<TickTime>, IComparable<long>
    {
        public TickTime Copy() => new TickTime(this.Ticks);

        public TickTime Add(long value, TickTime.Unit unit) => new TickTime(this.Ticks + (value * (long)unit));

        public TickTime AddMicroseconds(long value)
        {
            return this.Add(value, Unit.us);
        }

        public TickTime AddMilliseconds(long value)
        {
            return this.Add(value, Unit.ms);
        }

        public TickTime AddSeconds(long value)
        {
            return this.Add(value, Unit.s);
        }

        public TickTime AddMinutes(long value)
        {
            return this.Add(value, Unit.m);
        }

        public TickTime AddHours(long value)
        {
            return this.Add(value, Unit.h);
        }

        public TickTime AddDays(long value) => this.Add(value, Unit.d);

        public TickTime AddWeeks(long value) => this.Add(value, Unit.w);
    }
}
