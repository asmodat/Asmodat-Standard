
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AsmodatStandard.Extensions.Collections;

namespace AsmodatStandard.Extensions
{
    public static partial class ConsoleEx
    {
        public static string ReadPassword(int minLength = 0, int maxLength = int.MaxValue, string passwordCharReplacement = "*")
        {
            ConsoleKeyInfo k;
            var s = "";
            do
            {
                k = Console.ReadKey(true);
                var c = k.KeyChar;

                if (k.Key == ConsoleKey.Enter && s.Length >= minLength && s.Length <= maxLength)
                    break;

                if (k.Key == ConsoleKey.Backspace && s.Length > 0)
                {
                    ConsoleEx.DeleteCharacter();
                    s = s.Substring(0, s.Length - 1);
                    continue;
                }

                if (s.Length <= maxLength)
                {
                    Console.Write(passwordCharReplacement);
                    s += c;
                    continue;
                }

            } while (true);
            Console.WriteLine("");
            return s;
        }

        public static string ReadLineWhitelist(int minLength, int maxLength, bool caseIgnore, string charWhitelist)
            => ReadLineWhitelist(minLength: minLength, maxLength: maxLength, caseIgnore: caseIgnore, whitelist: charWhitelist?.ToArray());
        public static string ReadLineWhitelist(int minLength, int maxLength, bool caseIgnore, params char[] whitelist)
        {
            ConsoleKeyInfo k;
            var s = "";
            do
            {
                k = Console.ReadKey(true);
                var c = k.KeyChar;

                if (k.Key == ConsoleKey.Enter && s.Length >= minLength && s.Length <= maxLength)
                    break;

                if(k.Key == ConsoleKey.Backspace && s.Length > 0)
                {
                    ConsoleEx.DeleteCharacter();
                    s = s.Substring(0, s.Length - 1);
                    continue;
                }

                if(whitelist.Any(x => c.Equals(x, caseIgnore: caseIgnore) ) && s.Length <= maxLength)
                {
                    Console.Write(c);
                    s += c;
                    continue;
                }

            } while (true);
            Console.WriteLine("");
            return s;
        }


        public static int ConsoleWriteLine(this string s)
        {
            if (s.IsNullOrWhitespace())
                return 0;

            var backspaces = s.Count("\b");
            Console.WriteLine(s);
            return s.Length - backspaces;
        }

        public static ConsoleKeyInfo ReadUntilKey(bool intercept, params ConsoleKey[] keys)
        {
            ConsoleKeyInfo k;
            do
            {
                k = Console.ReadKey(intercept);
            } while (!keys.Any(x => x == k.Key));
            return k;
        }

        public static int ConsoleWrite(this string s)
        {
            if (s.IsNullOrWhitespace())
                return 0;

            var backspaces = s.Count("\b");
            Console.Write(s);
            return s.Length - backspaces;
        }

        public static int ConsoleWriteJson<T>(this T obj, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None)
        {
            if (obj == null)
                return 0;

            var s = "";
            if (typeof(T) == typeof(string) || typeof(T) == typeof(String))
                s = $"{obj}";
            else
                s = obj.JsonSerialize(formatting);

            return s.ConsoleWrite();
        }

        public static int WriteJsonLine<T>(T obj, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None)
        {
            if (obj == null)
                return 0;

            var s = "";
            if (typeof(T) == typeof(string) || typeof(T) == typeof(String))
                s = $"{obj}";
            else
                s = obj.JsonSerialize(formatting);

            return s.ConsoleWriteLine();
        }

        public static void DeleteCharacter()
        {
            Console.Write("\x1B[1D"); // Move the cursor one unit to the left
            Console.Write("\x1B[1P"); // Delete the character
        }
        public static void DeleteCharacters(int n)
        {
            for (int i = 0; i < n; i++)
                DeleteCharacter();
        }

        public static async Task<ConsoleKeyInfo?> ReadKey(int timeout, bool intercept = false)
        {
            var sw = Stopwatch.StartNew();
            while(sw.ElapsedMilliseconds < timeout)
            {
                if (Console.KeyAvailable)
                    return Console.ReadKey(intercept: intercept);

                await Task.Delay(10);
            }

            return null;
        }
    }
}
