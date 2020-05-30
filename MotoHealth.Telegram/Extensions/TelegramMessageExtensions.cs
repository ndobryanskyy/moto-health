using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Telegram.Extensions
{
    public static class TelegramMessageExtensions
    {
        public static bool TryExtractCommand(this Message message, [NotNullWhen(true)] out (string Command, string Arguments)? command)
        {
            command = null;

            var entities = message.Entities ?? new MessageEntity[0];
            var entityValues = message.EntityValues?.ToArray() ?? new string[0];
            var firstMessageEntity = entities.FirstOrDefault();

            var hasOneMessageEntity = entities.Length == 1 && entityValues.Length == 1;
            var firstEntityIsCommand = firstMessageEntity?.Offset == 0 && firstMessageEntity?.Type == MessageEntityType.BotCommand;

            var hasExactlyOneCommand = hasOneMessageEntity && firstEntityIsCommand;

            if (hasExactlyOneCommand)
            {
                var commandTokens = entityValues.First().Split('@', StringSplitOptions.RemoveEmptyEntries);
                var commandArguments = message.Text.Substring(firstMessageEntity!.Length).Trim();
                command = (commandTokens[0], commandArguments);
            }

            return hasExactlyOneCommand;
        }
    }
}