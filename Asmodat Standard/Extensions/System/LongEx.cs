using System;

namespace AsmodatStandard.Extensions
{
    public static class LongEx
    {
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
