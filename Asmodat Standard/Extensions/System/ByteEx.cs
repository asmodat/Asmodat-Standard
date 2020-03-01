using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsmodatStandard.Extensions
{
    public static class ByteEx
    {
        public static byte[] XOR(this byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                throw new Exception("In order to XOR 2 arrays they must have equal length.");

            var result = new byte[arr1.Length];
            for(int i = 0; i < arr1.Length; i++)
                result[i] = (byte)(arr1[i] ^ arr2[i]);

            return result;
        }

        public static string ToBase64String(this byte[] arr)
            => Convert.ToBase64String(arr);

        public static byte[] FromBase64String(this string str)
            => Convert.FromBase64String(str);

        public static byte[] TrimStart(this byte[] arr, byte b)
        {
            if (arr == null)
                return null;
            var newArr = new List<byte>();
            for (int i = 0; i < arr.Length; i++)
            {
                if (newArr.Count == 0 && arr[i] == b)
                    continue;

                newArr.Add(arr[i]);
            }

            return newArr.ToArray();
        }

        public static byte[] TrimEnd(this byte[] arr, byte b)
        {
            if (arr == null)
                return null;

            var newArr = new List<byte>();
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (newArr.Count == 0 && arr[i] == b)
                    continue;

                newArr.Add(arr[i]);
            }

            newArr.Reverse();
            return newArr.ToArray();
        }

        public static byte[] Trim(this byte[] arr, byte b) => arr.TrimStart(b).TrimEnd(b);
    }
}
