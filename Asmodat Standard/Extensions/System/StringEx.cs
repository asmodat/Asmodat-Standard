using AsmodatStandard.Extensions.Collections;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AsmodatStandard.Extensions
{
    public static class StringEx
    {
        public static MemoryStream ToMemoryStream(this string s, Encoding encoding = null)
            => new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(s));

        public static bool EquailsAny(this string s, StringComparison comparison, params string[] others)
        {
            if (others.IsNullOrEmpty())
                return false;

            foreach (var other in others)
                if ((other == null && s == null) || other.Equals(s, comparison))
                    return true;

            return false;
        }

        public static string ToHexString(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static byte[] HexToArray(this string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2, hex.Length - 2);

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static bool HexEquals(this string hex1, string hex2) => HexToArray(hex1).SequenceEqual(HexToArray(hex2));
        public static bool HexEquals(this string hex1, byte[] hex2) => HexToArray(hex1).SequenceEqual(hex2);
        public static bool HexEquals(this byte[] hex1, byte[] hex2) => hex1.SequenceEqual(hex2);
        public static bool HexEquals(this byte[] hex1, string hex2) => hex1.SequenceEqual(HexToArray(hex2));

        /// <summary>
        /// Converts string to byte array, default encoding is UTF8 if not specified
        /// </summary>
        public static byte[] ToByteArray(this string s, Encoding encoding = null)
            => (encoding ?? Encoding.UTF8).GetBytes(s);

        public static bool ToBoolOrDefault(this string s, bool @default = default(bool))
            => bool.TryParse(s, out var result) ? result : @default;

        public static int ToIntOrDefault(this string s, int @default = default(int))
            => int.TryParse(s, out var result) ? result : @default;

        public static long ToLongOrDefault(this string s, long @default = default(long))
            => long.TryParse(s, out var result) ? result : @default;

        public static double ToDoubleOrDefault(this string s, double @default = default(double))
            => double.TryParse(s,out var result) ? result : @default;

        public static string[] SplitByFirst(this string s, char c)
            => s.Split(new char[] { c }, 2);

        public static string Replace(this string str, params (string to, string with)[] replace)
        {
            replace.ForEach(r => str = str.Replace(r.to, r.with));
            return str;
        }

        /// <summary>
        /// Math Min of GZip'ed ShannonEntropy and ShannonEntropy on raw string, should provide more accurate entropy value on longer strings
        /// </summary>
        public static double ShannonGZipEntropy(this string str)
            => Math.Min(str.GZip(encoding: Encoding.UTF8, level: CompressionLevel.Optimal).ShannonEntropy(), str.ShannonEntropy());
 
        public static double ShannonEntropy(this string str)
        {
            var map = str.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            var result = 0.0;
            var len = str.Length;

            foreach (var item in map)
            {
                var frequency = (double)item.Value / len;
                result += frequency * Math.Log(frequency, 2);
            }

            return -result;
        }

        public static string GZip(this string str, Encoding encoding = null, CompressionLevel level = CompressionLevel.Optimal)
        {
            encoding = encoding ?? Encoding.UTF8;
            var buffer = encoding.GetBytes(str);
            var memory = new MemoryStream();

            using (GZipStream stream = new GZipStream(memory, level, true))
                stream.Write(buffer, 0, buffer.Length);

            memory.Position = 0;
            byte[] data = new byte[memory.Length];
            memory.Read(data, 0, data.Length);

            byte[] zipbuffer = new byte[data.Length + 4];
            Buffer.BlockCopy(data, 0, zipbuffer, 4, data.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, zipbuffer, 0, 4);
            return Convert.ToBase64String(zipbuffer);
        }

        public static string UnGZip(this string str, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            var zipbuffer = Convert.FromBase64String(str);

            using (MemoryStream memory = new MemoryStream())
            {
                int length = BitConverter.ToInt32(zipbuffer, 0);
                memory.Write(zipbuffer, 4, zipbuffer.Length - 4);
                memory.Position = 0;

                var buffer = new byte[length];
                using (GZipStream stream = new GZipStream(memory, CompressionMode.Decompress))
                    stream.Read(buffer, 0, buffer.Length);

                return encoding.GetString(buffer);
            }
        }

        public static string CoalesceIfEquals(this string str, string val, string replaceWith) => str == val ? replaceWith : str;
        public static string CoalesceNullOrEmpty(this string str, string value) => str.IsNullOrEmpty() ? value : str;
        public static string CoalesceNullOrWhitespace(this string str, string value) => str.IsNullOrWhitespace() ? value : str;

        public static string ReplaceNonASCII(this string str, string replaceWith) => Regex.Replace(str, @"[^\u0000-\u007F]+", replaceWith);

        public static string SkipChars(this string str, int count) => str.Substring(count, str.Length - count);

        public static bool StartsWithAny(this string str, params string[] any) => any?.Any(s => str.StartsWith(s)) == true;

        public static bool EndsWithAny(this string str, params string[] any) => any?.Any(s => str.EndsWith(s)) == true;

        public static bool IsEmpty(this string str) => str == string.Empty;

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsNullOrWhitespace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool Equals(this string left, string right, StringComparison stringComparison) => string.Equals(left, right, stringComparison);
    }
}
