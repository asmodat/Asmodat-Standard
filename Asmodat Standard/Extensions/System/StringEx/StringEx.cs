using AsmodatStandard.Extensions.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AsmodatStandard.Extensions
{
    public static partial class StringEx
    {
        public static string Base64Encode(this string s, System.Text.Encoding encoding = null) => System.Convert.ToBase64String((encoding ?? System.Text.Encoding.UTF8).GetBytes(s));
        public static string Base64Decode(this string s, System.Text.Encoding encoding = null) => (encoding ?? Encoding.UTF8).GetString(System.Convert.FromBase64String(s));

        /// <summary>
        /// Checks is string is a valid base 64 string
        /// </summary>
        /// <param name="s">base 64 string</param>
        /// <returns>true if string is base64 encoded, otherwise false</returns>
        public static bool IsBase64(this string s)
        {
            if (s.IsNullOrWhitespace())
                return false;

            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static string ConcatNullOrEmpty(this string str1, string str2) => str1.IsNullOrEmpty() ? str2 : str1;
        public static string ConcatNullOrWhitespace(this string str1, string str2) => str1.IsNullOrWhitespace() ? str2 : str1;        public static string TrimOrDefault(this string s, char trim, string @default = default(string))
        {
            if (s == null)
                return @default;

            try
            {
                return s.Trim(trim);
            }
            catch
            {
                return @default;
            }
        }

        public static async Task<T> JsonDeserializeAsync<T>(this Task<string> json)
            => JsonConvert.DeserializeObject<T>(await json);

        public static T JsonDeserialize<T>(this string json)
            => JsonConvert.DeserializeObject<T>(json);

        public static string Reverse(this string s)
        {
            if (s.IsNullOrEmpty())
                return s;

            var arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static IEnumerable<string> DistinctSubstrings(this string s)
        {
            if (s == null)
                return null;

            if (s.IsEmpty())
                return new string[0];

            List<string> list = new List<string>();
            for (int i = 0; i < s.Length; i++)
                for (int j = i; j < s.Length; j++)
                {
                    string ss = s.Substring(i, j - i + 1);
                    list.Add(ss);
                }

            return list.Distinct().ToArray();
        }

        /// <summary>
        /// http://www.asciitable.com/
        /// True if all characters are in [32, 126] decimal range
        /// </summary>
        public static bool AllVisibleAscii(this string s)
        {
            if (s.IsNullOrEmpty())
                return false;

            if (s.Any(c => c < 32 || c > 126))
                return false;

            return true;
        }

        /// <summary>
        /// http://www.asciitable.com/
        /// True is string has special ASCII characters in following ranges:
        /// {[32-47], [58-64], [91,96], [123-126]}
        /// </summary>
        public static bool AnySpecialAscii(this string s)
        {
            if (s.IsNullOrEmpty())
                return false;

            if (s.Any(c => 
            (c >= 32 && c <= 47) || 
            (c >= 58 && c <= 64) || 
            (c >= 91 && c <= 96) || 
            (c >= 123 && c <= 126)))
                return true;

            return false;
        }

        public static bool AnyDigits(this string s)
            => !s.IsNullOrEmpty() && s.Any(char.IsDigit);

        public static bool AnyUpper(this string s)
            => !s.IsNullOrEmpty() && s.Any(char.IsUpper);

        public static bool AnyLower(this string s)
            => !s.IsNullOrEmpty() && s.Any(char.IsLower);

        public static bool IsAlphaNumericAscii(this string s)
            => !s.IsNullOrEmpty() && new Regex("^[a-zA-Z][a-zA-Z0-9]*$").IsMatch(s);

        public static bool IsAlphanumericUnicode(this string s)
            => !s.IsNullOrEmpty() && s.Any(char.IsLetterOrDigit);

        /// <summary>
        /// Splits sting by character but allows for escaping
        /// </summary>
        public static string[] EscapedSplit(this string s, char splitChar, char escapeChar = '\\')
        {
            var escapedSequence = $"{escapeChar}{splitChar}";
            var replacement = Guid.NewGuid().ToString().ReplaceMany("", $"{splitChar}", $"{escapeChar}", "-");
            var newString = s.Replace(escapedSequence, replacement);
            var result = newString.Split(splitChar);
            for (var i = 0; i < result.Length; i++)
                result[i] = result[i].Replace(replacement, $"{splitChar}");

            return result;
        }

        /// <summary>
        /// Repeats string n times
        /// </summary>
        public static string Repeat(this string s, int n)
        {
            if (n < 0)
                throw new ArgumentException("Sequence can't be repeated negative times.");

            var result = string.Empty;
            for (int i = 0; i < n; i++)
                result += s;

            return result;
        }

        public static int Count(this string s, string pattern)
        {
            if (s.IsNullOrEmpty())
                return 0;

            if (pattern.IsNullOrEmpty())
                throw new ArgumentException("Count pattern can't be null or empty.");

            int count = 0, i = 0;
            while ((i = s.IndexOf(pattern, i)) != -1)
            {
                i += pattern.Length;
                count++;
            }
            return count;
        }

        public static string Trim(this string target, string trim, int count = int.MaxValue)
        {
            if (trim.IsNullOrEmpty())
                return target;

            target = target.TrimStart(trim, count: count);

            if (trim.IsNullOrEmpty())
                return target;

            return target.TrimEnd(trim, count: count);
        }

        public static string TrimStartSingle(this string target, string trim)
            => target.TrimStart(trim, count: 1);

        public static string TrimEndSingle(this string target, string trim)
            => target.TrimStart(trim, count: 1);

        public static string TrimStart(this string target, string trim, int count = int.MaxValue)
        {
            if (trim.IsNullOrEmpty())
                return target;

            var result = target;
            while (--count >= 0 && result.StartsWith(trim))
                result = result.Substring(trim.Length);

            return result;
        }

        public static string TrimEnd(this string target, string trim, int count = int.MaxValue)
        {
            if (trim.IsNullOrEmpty())
                return target;

            var result = target;
            while (--count >= 0 && result.EndsWith(trim))
                result = result.Substring(0, result.Length - trim.Length);

            return result;
        }

        public static bool StartsAndEndsWith(this string str, string subStr)
            => str.StartsWith(subStr) && str.EndsWith(subStr);

        public static bool StartsOrEndsWith(this string str, string subStr)
            => str.StartsWith(subStr) || str.EndsWith(subStr);

        public static string SetChar(this string s, int index, char c)
        {
            var arr = s.ToCharArray();
            arr[index] = c;
            return new string(arr);
        }

        public static string ReplaceMany(this string str, char newValue, params char[] oldValues)
            => str.ReplaceMany(newValue.ToString(), oldValues);

        public static string ReplaceMany(this string str, char newValue, params string[] oldValues)
            => str.ReplaceMany(newValue.ToString(), oldValues);

        public static string ReplaceMany(this string str, string newValue, params string[] oldValues)
            => oldValues.ForEachWithPrevious((s, previous) => previous.Replace(s, newValue), str);

        public static string ReplaceMany(this string str, string newValue, params char[] chars)
            => chars.ForEachWithPrevious((c, previous) => previous.Replace(c.ToString(), newValue), str);

        public static string ReplaceMany(this string str, params (string oldStr, string newStr)[] reps)
        {
            var result = str;
            reps.ForEach(r => result = result.Replace(r.oldStr, r.newStr));
            return result;
        }

        public static string TrimStartMany(this string str, IEnumerable<string> trim)
            => str.TrimStartMany(trim.ToArray());

        public static string TrimStartMany(this string str, params string[] trim)
        {
            var result = str;
            foreach (var sub in trim)
            {
                result = result.TrimStart(sub);

                if (result.IsNullOrEmpty())
                    return result;
            }

            return result;
        }

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

        public static bool Contains(this string s, string substring, CompareOptions compareOptions, CultureInfo cultureInfo = null) =>
            !s.IsNullOrEmpty() && !substring.IsNullOrEmpty() &&
            (cultureInfo ?? CultureInfo.CurrentCulture)
            .CompareInfo.IndexOf(s, substring, compareOptions) >= 0;

        public static bool ContainsAny(this string s, params string[] others)
            => ContainsAny(s: s, others: others?.ToIEnumerable());

        public static bool ContainsAny(this string s, IEnumerable<string> others)
        {
            if (others.IsNullOrEmpty())
                return false;

            foreach (var other in others)
                if ((other == null && s == null) || s.Contains(other))
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

        public static int HexToInt(this string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2, hex.Length - 2);

            return int.Parse(hex, NumberStyles.HexNumber);
        }

        public static long HexToLong(this string hex)
        {
            if (hex.StartsWith("0x"))
                hex = hex.Substring(2, hex.Length - 2);

            return long.Parse(hex, NumberStyles.HexNumber);
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

        public static bool ToBool(this string s)  => bool.Parse(s);

        public static bool ToBoolOrDefault(this string s, bool @default = default(bool))
            => bool.TryParse(s, out var result) ? result : @default;

        public static int ToIntOrDefault(this string s, int @default = default(int))
            => int.TryParse(s, out var result) ? result : @default;

        public static long ToLongOrDefault(this string s, long @default = default(long))
            => long.TryParse(s, out var result) ? result : @default;

        public static double ToDoubleOrDefault(this string s, double @default = default(double))
            => double.TryParse(s,out var result) ? result : @default;

        /// <summary>
        /// Splits sting by the first occurence of 'c'
        /// </summary>
        public static string[] SplitByFirst(this string s, char c)
            => s.Split(new char[] { c }, 2);

        public static string[] SplitByLast(this string s, char c)
        {
            int idx = s.LastIndexOf(c);

            if ((idx + 1) == s.Length)
                return new string[] { s.Substring(0, idx), null };

            if (idx != -1)
                return new string[] { s.Substring(0, idx), s.Substring(idx + 1) };
            else
                return new string[] { s };
        }

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
