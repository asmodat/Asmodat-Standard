
namespace AsmodatStandard.Extensions
{
    public static partial class CharEx
    {
        public static readonly char[] SpecialAscii = new char[] {
            '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.','/',
            ':', ';', '<', '=', '>', '?', '@',
            '[', '\\',']', '^', '_', '`',
            '{', '|', '}', '~'
        };

        public static bool IsDigit(this char c) => char.IsDigit(c);
        public static bool IsUpper(this char c) => char.IsUpper(c);
        public static char ToLower(this char c) => char.ToLower(c);
        public static char ToUpper(this char c) => char.ToUpper(c);
    }
}
