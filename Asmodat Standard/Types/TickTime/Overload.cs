using System;

namespace AsmodatStandard.Types
{
    //TODO: IFormattable,

    /// <summary>
    /// This class stores and manages DateTime as UTC tick long value, it is safe and efficient to use with serialization instead of DateTime
    /// </summary>
    public partial struct TickTime : IComparable, IEquatable<TickTime>, IEquatable<long>, IComparable<TickTime>, IComparable<long>
    {
        #region Math Operators
        public static TickTime operator +(TickTime x, long y)
        {
            return new TickTime(x.Ticks + y);
        }
        public static TickTime operator -(TickTime x, long y)
        {
            return new TickTime(x.Ticks - y);
        }
        public static TickTime operator *(TickTime x, long y)
        {
            return new TickTime(x.Ticks * y);
        }
        public static TickTime operator /(TickTime x, long y)
        {
            return new TickTime(x.Ticks * y);
        }
        public static bool operator ==(TickTime x, long y)
        {
            return x.Ticks == y;
        }
        public static bool operator !=(TickTime x, long y)
        {
            return x.Ticks == y;
        }
        public static bool operator >(TickTime x, long y)
        {
            return x.Ticks > y;
        }
        public static bool operator <(TickTime x, long y)
        {
            return x.Ticks < y;
        }
        public static bool operator >=(TickTime x, long y)
        {
            return x.Ticks >= y;
        }
        public static bool operator <=(TickTime x, long y)
        {
            return x.Ticks <= y;
        }

        public static TickTime operator ++(TickTime x)
        {
            return new TickTime(++x.Ticks);
        }
        public static TickTime operator --(TickTime x)
        {
            return new TickTime(--x.Ticks);
        }

        public static TickTime operator +(TickTime x, TickTime y)
        {
            return new TickTime(x.Ticks + y.Ticks);
        }
        public static TickTime operator -(TickTime x, TickTime y)
        {
            return new TickTime(x.Ticks - y.Ticks);
        }
        public static TickTime operator *(TickTime x, TickTime y)
        {
            return new TickTime(x.Ticks * y.Ticks);
        }
        public static TickTime operator /(TickTime x, TickTime y)
        {
            return new TickTime(x.Ticks * y.Ticks);
        }
        public static bool operator ==(TickTime x, TickTime y)
        {
            return x.Ticks == y.Ticks;
        }
        public static bool operator !=(TickTime x, TickTime y)
        {
            return x.Ticks != y.Ticks;
        }
        public static bool operator >(TickTime x, TickTime y)
        {
            return x.Ticks > y.Ticks;
        }
        public static bool operator <(TickTime x, TickTime y)
        {
            return x.Ticks < y.Ticks;
        }
        public static bool operator >=(TickTime x, TickTime y)
        {
            return x.Ticks >= y.Ticks;
        }
        public static bool operator <=(TickTime x, TickTime y) => x.Ticks <= y.Ticks;
        
        #endregion

        public static implicit operator DateTime(TickTime TT) => TT.UTC;
        
        public static implicit operator long(TickTime TT) => TT.Ticks;
        
        public static implicit operator TickTime(DateTime DT) => new TickTime(DT);

        public static implicit operator TickTime(long ticks) => new TickTime(ticks);
    }
}
