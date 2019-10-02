using System;

namespace AsmodatStandard.Extensions
{
    public static class LongEx
    {
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
