namespace MotoHealth.Bot.Extensions
{
    internal static class StringExtensions
    {
        public static string ToCamelCase(this string source) 
            => $"{char.ToLowerInvariant(source[0])}{source[1..]}";
    }
}