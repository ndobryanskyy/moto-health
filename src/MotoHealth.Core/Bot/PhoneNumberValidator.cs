using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MotoHealth.Core.Bot
{
    public interface IPhoneNumberParser
    {
        bool TryParse(string input, [NotNullWhen(true)] out string? phoneNumber);
    }

    internal sealed class PhoneNumberParser : IPhoneNumberParser
    {
        private static readonly Regex IgnoredCharactersRegex = new Regex(@"[\s-)(]", RegexOptions.Compiled);
        private static readonly Regex PhoneNumberRegex = new Regex(@"(^\d{9}$)|(^\d{10}$)|(?:^\+?(\d{12}$))", RegexOptions.Compiled);

        public bool TryParse(string input, [NotNullWhen(true)] out string? phoneNumber)
        {
            phoneNumber = null;

            var normalized = IgnoredCharactersRegex.Replace(input, "");

            var match = PhoneNumberRegex.Match(normalized);

            if (!match.Success)
            {
                return false;
            }

            phoneNumber = match switch
            {
                { Groups: var groups } when groups[1].Success => $"+380{groups[1].Value}",
                { Groups: var groups } when groups[2].Success => $"+38{groups[2].Value}",
                { Groups: var groups } when groups[3].Success => $"+{groups[3].Value}",
                _ => throw new ArgumentOutOfRangeException()
            };

            return true;
        }
    }
}