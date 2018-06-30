using System;

namespace AsmodatStandard.Extensions
{
    public static class Intervals
    {
        public static bool InClosedInterval<T>(this T t, T min, T max) where T : IComparable<T>, IEquatable<T>
        {
            if (t.CompareTo(min) >= 0 && t.CompareTo(max) <= 0)
                return true;

            return false;
        }
    }
}
