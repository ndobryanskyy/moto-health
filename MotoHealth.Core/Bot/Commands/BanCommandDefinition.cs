using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Commands
{
    public sealed class BanCommandDefinition : CommandDefinition
    {
        private static readonly Regex WhitespaceSplitRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public BanCommandDefinition(string command) 
            : base(command)
        {
        }

        public bool Matches(ICommandMessageBotUpdate commandMessage, [NotNullWhen(true)] out (string Secret, int UserId)? arguments)
        {
            arguments = null;

            if (Matches(commandMessage))
            {
                var argumentsSplit = WhitespaceSplitRegex
                    .Split(commandMessage.Arguments)
                    .Where(x => x != string.Empty)
                    .ToArray();
                
                if (argumentsSplit.Length == 2 && int.TryParse(argumentsSplit[1], out var userId))
                {
                    arguments = (argumentsSplit[0], userId);

                    return true;
                }
            }

            return false;
        }
    }
}