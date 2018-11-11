using System.Linq;

namespace HLang.Helper
{
    static class CharHelper
    {
        public static bool IsHex(this char c) => char.IsDigit(c) || char.ToLower(c) >= 'a' && char.ToLower(c) <= 'f';
        public static bool IsOctal(this char c) => c >= '0' && c <= '7';
        public static bool IsBin(this char c) => c == '0' || c == '1';
        public static bool IsBasePrefix(this char c) => new[] { 'x', 'o', 'b' }.Contains(char.ToLower(c));
        public static bool IsIdentifier(this char c) => char.IsLetterOrDigit(c) || c == '_';
        public static bool IsNumberLiteral(this char c) => char.IsDigit(c) || c == '_';
        public static bool IsUniqueRealLiteral(this char c) => new[] { 'e', 'E', '.' }.Contains(char.ToLower(c));
    }
}
