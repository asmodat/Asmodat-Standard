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
        public static string Replace(this string str, params (string to, string with)[] replace)
        {
            replace.ForEach(r => str = str.Replace(r.to, r.with));
            return str;
        }


        /// <summary>
        /// Math Min of GZip'ed ShannonEntropy and ShannonEntropy on raw string, should provide more accurate entropy value on longer strings
        /// </summary>
        public static double ShannonGZipEntropy(this string str)
            => Math.Min(str.GZip().ShannonEntropy(), str.ShannonEntropy());
 
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
