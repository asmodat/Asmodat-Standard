using System;
using System.Text.RegularExpressions;

namespace AsmodatStandard.Extensions
{
    public static class GuidEx
    {
        public static bool IsGuid(this string str) 
            => !str.IsNullOrWhitespace() && Guid.TryParse(str, out var guid);

        public static string SlimUID()
            => Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", "");
    }
}
