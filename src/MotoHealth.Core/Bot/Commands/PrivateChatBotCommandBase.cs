using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot.Commands
{
    internal abstract class PrivateChatBotCommandBase : BotCommandBase
    {
        protected PrivateChatBotCommandBase(CommandName name) 
            : base(name)
        {
        }

        protected override bool Matches(ICommandMessageBotUpdate commandMessage)
        {
            return base.Matches(commandMessage) &&
                   commandMessage.Chat.Type == ChatType.Private;
        }
    }
}