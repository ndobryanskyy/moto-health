using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using MotoHealth.Bot.Telegram.Updates;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Bot.Telegram
{
    public interface IBotUpdateResolver
    {
        bool TryResolveSupportedUpdate(Update update, [NotNullWhen(true)] out IBotUpdate? supportedUpdate);
    }

    internal sealed class BotUpdateResolver : IBotUpdateResolver
    {
        private readonly Regex _whitespaceRegex = new Regex(@"\s+");

        public bool TryResolveSupportedUpdate(Update update, out IBotUpdate? supportedUpdate)
        {
            supportedUpdate = update.Type switch
            {
                UpdateType.Message => ResolveMessageBotUpdate(update),
                _ => null
            };

            return false;
        }

        private IMessageBotUpdate? ResolveMessageBotUpdate(Update update)
        {
            var message = update.Message ?? throw new ArgumentException($"{nameof(update.Message)} must be available!", nameof(update));

            return message.Type switch
            {
                MessageType.Text when TryCreateCommandUpdate(update, message, out var commandUpdate) => commandUpdate,
                MessageType.Text => new TextMessageBotUpdate(update, message.Text),
                _ => null
            };
        }

        private bool TryCreateCommandUpdate(
            Update update,
            Message message, 
            [NotNullWhen(true)] out ICommandBotUpdate? botUpdate)
        {
            botUpdate = null;

            if (message.Entities.Length == 1)
            {
                var messageEntity = message.Entities[0];

                if (messageEntity.Type == MessageEntityType.BotCommand &&
                    messageEntity.Offset == 0)
                {
                    var command = ParseCommand(message.EntityValues.FirstOrDefault());
                    var arguments = _whitespaceRegex.Split(message.Text.Substring(messageEntity.Length));

                    botUpdate = new CommandBotUpdate(update, command, arguments);
                    return true;
                }
            }

            return false;
        }

        private static BotCommand ParseCommand(string? command)
        {
            return command?.ToLowerInvariant() switch
            {
                "start" => BotCommand.Start,
                "accident" => BotCommand.ReportAccident,
                "about" => BotCommand.About,
                _ => BotCommand.Unknown
            };
        }
    }
}