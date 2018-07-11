using System;

namespace AsmodatStandard.Extensions
{
    public static class GuidEx
    {
        public static bool IsGuid(this string str) 
            => !str.IsNullOrWhitespace() && Guid.TryParse(str, out var guid);
    }
}
