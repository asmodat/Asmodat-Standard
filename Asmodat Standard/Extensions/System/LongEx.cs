using System;
using System.Diagnostics;
using System.Linq;

namespace AsmodatStandard.Extensions
{
    public static class LongEx
    {
        public static string ToHexString(this long l) => string.Format("{0:X}", l);
        
        /// <summary>
        /// shift right base 10
        /// e.g. 123456789.SHR10(5) == 1234
        /// </summary>
        public static long SHR10(this long l, int n)
        {
            var s = l.ToString();
            if (l > 0 && s.Length <= n)
                return 0;
            else if (l < 0 && s.Length <= (n + 1))
                return 0;

            return s.Substring(0, s.Length - n).ToLong();
        }

        /// <summary>
        /// shift left base 10
        /// e.g. 1234.SHL10(5) == 123400000
        /// </summary>
        public static long SHL10(this long l, int n)
        {
            if (l == 0)
                return 0;

            return $"{l}{"0".Repeat(n)}".ToLong();
        }

        public static string ToPrettyTimeSpan(this Stopwatch sw)
        {
            var s = (sw?.Elapsed)?.TotalSeconds ?? 0;
            if (s.IsInfinity() || s.IsNaN() || s >= long.MaxValue || s <= 0)
                return ((long)0).ToPrettyTimeSpan();

            return ((long)s).ToPrettyTimeSpan();
        }

        public static string ToPrettyTimeSpan(this long seconds)
        {
            if (seconds <= 0)
                return "0 Seconds";

            TimeSpan t = TimeSpan.FromSeconds(seconds);
            
            var result = "";

            if (t.Days > 0)
                result += (t.Days == 1) ? "1 Day " : $"{t.Days} Days ";

            if (t.Hours > 0)
                result += (t.Hours == 1) ? "1 Hour " : $"{t.Hours} Hours ";

            if (t.Minutes > 0)
                result += (t.Minutes == 1) ? "1 Minute " : $"{t.Minutes} Minutes ";

            if (t.Seconds > 0)
                result += (t.Seconds == 1) ? "1 Second" : $"{t.Seconds} Seconds";

            return result.Trim();
        }

        public static string ToPrettyBytes(this long bytes)
        {
            if (bytes < 0)
                return "NaN B";

            var units = new string[] { "B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (bytes < Math.Pow(1024, 1))
                return $"{bytes} {units[0]}";

            for (int i = 1; i < units.Length; i++)
            {
                if(bytes < Math.Pow(1024, i + 1))
                    return $"{string.Format("{0:0.000}", bytes / Math.Pow(1024, i))} {units[i]}";
            }

            var endIndex = units.Length - 1;
            return $"{string.Format("{0:0.000}", bytes / Math.Pow(1024, endIndex))} {units[endIndex]}";
        }

        public static byte[] ToByteArray(this long l) => BitConverter.GetBytes(l);

        public static double Average(this long[] input)
        {
            long sum = input[0];
            for (int i = 1; i < input.Length; i++)
                sum += input[i];

            return sum / input.Length;
        }

        public static long Max(this long[] input)
        {
            long output = input[0];
            for (int i = 1; i < input.Length; i++)
                if (input[i] > output)
                    output = input[i];

            return output;
        }

        public static long Min(this long[] input)
        {
            long output = input[0];
            for (int i = 1; i < input.Length; i++)
                if (input[i] < output)
                    output = input[i];

            return output;
        }
    }
}
