using System.Globalization;
using ScrapySharp.Html.Parsing;

namespace ScrapySharp.Extensions
{
    public static class TokenHelper
    {
        public static bool IsToken(this Word word)
        {
            return IsToken(word.Value);
        }
        
        public static bool IsToken(this char c)
        {
            return c == Tokens.TagBegin ||
                c == Tokens.TagEnd ||
                c == Tokens.Quote ||
                c == Tokens.SimpleQuote;
        }

        public static bool IsToken(this string value)
        {
            if (value.Length <= 0)
                return false;

            if (value.Length == 1)
                return IsToken(value[0]);

            return value == Tokens.CloseTag ||
                   value == Tokens.CommentBegin ||
                   value == Tokens.CommentEnd ||
                   value == Tokens.Doctype ||
                   value == Tokens.CloseTagDeclarator;
        }
    }
}