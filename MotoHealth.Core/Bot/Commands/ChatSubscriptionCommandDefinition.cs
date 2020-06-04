using System.Diagnostics.CodeAnalysis;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Commands
{
    public sealed class ChatSubscriptionCommandDefinition : CommandDefinition
    {
        public ChatSubscriptionCommandDefinition(string command) 
            : base(command)
        {
        }
        
        public bool Matches(ICommandMessageBotUpdate commandMessage, [NotNullWhen(true)] out string? subscriptionSecret)
        {
            subscriptionSecret = null;

            if (!Matches(commandMessage))
            {
                return false;
            }
            else
            {
                subscriptionSecret = commandMessage.Arguments;
                return true;
            }
        }
    }
}