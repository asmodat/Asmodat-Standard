using AsmodatStandard.Types;

namespace AsmodatStandard.Extensions.Types
{
    public static class TickTimeEx
    {
        public static TickTime ToTickTime(this long ticks)
            => new TickTime(ticks);

        public static bool Timeout(this TickTime start, long timeout_ms)
            => TickTime.Timeout(start, timeout_ms, TickTime.Unit.ms);

        public static bool Timeout(this TickTime start, long timeout, TickTime.Unit unit = TickTime.Unit.ms)
            => TickTime.Timeout(start, timeout, unit);

        public static decimal TimeoutSpan(this TickTime start, long timeout, TickTime.Unit unit = TickTime.Unit.ms)
            => TickTime.TimeoutSpan(start, timeout, unit, unit);

        public static decimal TimeoutSpan(this TickTime start, long timeout, TickTime.Unit unit, TickTime.Unit unit_result) 
            => TickTime.TimeoutSpan(start, timeout, unit, unit_result);

        public static int GetBase(this TickTime.Unit unit)
        {
            switch (unit)
            {
                case TickTime.Unit.us: return 1000;
                case TickTime.Unit.ms: return 1000;
                case TickTime.Unit.s: return 60;
                case TickTime.Unit.m: return 60;
                case TickTime.Unit.h: return 24;
                case TickTime.Unit.d: return 7;
                case TickTime.Unit.w: return 1;
                default: return -1;
            }
        }
    }
}