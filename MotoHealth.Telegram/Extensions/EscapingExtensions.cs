using System;
using System.Text.RegularExpressions;

namespace MotoHealth.Telegram.Extensions
{
    public static class EscapingExtensions
    {
        private static readonly Regex TelegramHtmlSpecialCharacters = new Regex(@"[<>&]", RegexOptions.Compiled);

        public static string HtmlEscaped(this string input)
            => TelegramHtmlSpecialCharacters.Replace(input, EscapeSymbol);

        private static string EscapeSymbol(Match match)
        {
            return match.Value switch
            {
                "&" => "&amp;",
                ">" => "&gt;",
                "<" => "&lt;",
                _ => throw new ArgumentOutOfRangeException($"Matched unexpected character: {match.Value}")
            };
        }
    }
}