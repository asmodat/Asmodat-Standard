
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

        public static bool EqualsCaseIgnore(this char c1, char c2) => c1.ToLower().Equals(c2.ToLower());
        public static bool Equals(this char c1, char c2, bool caseIgnore) => caseIgnore ? c1.EqualsCaseIgnore(c2) : c1.Equals(c2);

        public static bool IsDigit(this char c) => char.IsDigit(c);
        public static bool IsUpper(this char c) => char.IsUpper(c);
        public static char ToLower(this char c) => char.ToLower(c);
        public static char ToUpper(this char c) => char.ToUpper(c);

        public static bool IsAsciiLetter(this char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        public static bool IsAsciiLetterOrNumber(this char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
    }
}
