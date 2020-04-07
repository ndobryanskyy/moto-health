using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using BotCommand = MotoHealth.Core.Bot.Updates.BotCommand;

namespace MotoHealth.Bot.Telegram
{
    public interface IBotUpdateResolver
    {
        bool TryResolveSupportedUpdate(Update update, [NotNullWhen(true)] out IBotUpdate? supportedUpdate);
    }

    internal sealed class BotUpdateResolver : IBotUpdateResolver
    {
        private readonly Regex _whitespaceRegex = new Regex(@"\s+");

        private readonly IMapper _mapper;

        public BotUpdateResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public bool TryResolveSupportedUpdate(Update update, out IBotUpdate? supportedUpdate)
        {
            supportedUpdate = update switch
            {
                { Type: UpdateType.Message, Message: Message message } => ResolveMessageBotUpdate(message),
                _ => null
            };

            return supportedUpdate != null;

            IMessageBotUpdate? ResolveMessageBotUpdate(Message message)
            {
                return message switch
                {
                    { Type: MessageType.Contact, Contact: Contact contact } => new ContactMessageBotUpdate(update.Id, message.MessageId, GetChatFromMessage(message), _mapper.Map<TelegramContact>(contact)),
                    { Type: MessageType.Text } when TryCreateCommandUpdate(out var commandUpdate) => commandUpdate,
                    { Type: MessageType.Text } => new TextMessageBotUpdate(update.Id, message.MessageId, GetChatFromMessage(message), message.Text),
                    _ => null
                };

                bool TryCreateCommandUpdate(out ICommandBotUpdate? botUpdate)
                {
                    botUpdate = null;

                    if (message.Entities?.Length == 1)
                    {
                        var messageEntity = message.Entities[0];

                        if (messageEntity.Type == MessageEntityType.BotCommand &&
                            messageEntity.Offset == 0)
                        {
                            var command = ParseCommand(message.EntityValues.FirstOrDefault());
                            var arguments = _whitespaceRegex.Split(message.Text.Substring(messageEntity.Length))
                                .Where(x => !string.IsNullOrEmpty(x))
                                .ToArray();

                            botUpdate = new CommandBotUpdate(update.Id, message.MessageId, GetChatFromMessage(message), command, arguments);
                            return true;
                        }
                    }

                    return false;
                }
            }
        }

        private TelegramChat GetChatFromMessage(Message message)
        {
            var isGroup = message.Chat.Type != ChatType.Private;

            var from = _mapper.Map<TelegramUser>(message.From);
            var group = isGroup ? _mapper.Map<TelegramGroup>(message.Chat) : null;

            return new TelegramChat(message.Chat.Id, from, group);
        }

        private static BotCommand ParseCommand(string? command)
        {
            return command?.ToLowerInvariant() switch
            {
                "/start" => BotCommand.Start,
                "/dtp" => BotCommand.ReportAccident,
                "/info" => BotCommand.About,
                _ => BotCommand.Unknown
            };
        }
    }
}