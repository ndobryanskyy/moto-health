using System.Text.RegularExpressions;

namespace MotoHealth.Core.Extensions
{
    internal static class MarkdownExtensions
    {
        private static readonly Regex ReservedCharacters = new Regex(@"[_*\][)(~`>#+=|}{.!\\-]", RegexOptions.Compiled);

        public static string EscapeForMarkdown(this string input) 
            => ReservedCharacters.Replace(input, EscapeSymbol);

        private static string EscapeSymbol(Match match)
        {
            return $"\\{match.Value}";
        }
    }
}