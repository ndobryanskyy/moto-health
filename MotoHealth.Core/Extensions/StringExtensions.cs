namespace MotoHealth.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string source) 
            => $"{char.ToLowerInvariant(source[0])}{source[1..]}";
    }
}