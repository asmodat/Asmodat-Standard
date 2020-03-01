using System;

namespace AsmodatStandard.Extensions
{
    public static class ULongEx
    {
        public static string ToPrettyBytes(this ulong bytes)
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

        public static byte[] ToByteArray(this ulong l) => BitConverter.GetBytes(l);

        public static double Average(this ulong[] input)
        {
            var sum = input[0];
            for (int i = 1; i < input.Length; i++)
                sum += input[i];

            return (double)sum / input.Length;
        }

        public static ulong Max(this ulong[] input)
        {
            var output = input[0];
            for (int i = 1; i < input.Length; i++)
                if (input[i] > output)
                    output = input[i];

            return output;
        }

        public static ulong Min(this ulong[] input)
        {
            var output = input[0];
            for (int i = 1; i < input.Length; i++)
                if (input[i] < output)
                    output = input[i];

            return output;
        }
    }
}
