using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AutoMapper;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;
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
        private readonly IMapper _mapper;

        public BotUpdateResolver(IMapper mapper)
        {
            _mapper = mapper;
        }

        public bool TryResolveSupportedUpdate(Update update, out IBotUpdate? supportedUpdate)
        {
            supportedUpdate = update switch
            {
                { Type: UpdateType.Message, Message: Message _ } => ResolveMessageBotUpdate(update),
                _ => null
            };

            return supportedUpdate != null;
        }

        private IMessageBotUpdate? ResolveMessageBotUpdate(Update update)
        {
            var message = update.Message ?? throw new ArgumentException(nameof(update));

            return message switch
            {
                { Type: MessageType.Contact, Contact: Contact _ } => _mapper.Map<ContactMessageBotUpdate>(update),
                { Type: MessageType.Text } when HasOnlyOneCommandEntity(message) => _mapper.Map<CommandMessageBotUpdate>(update),
                { Type: MessageType.Text } => _mapper.Map<TextMessageBotUpdate>(update),
                _ => null
            };
        }

        private bool HasOnlyOneCommandEntity(Message message)
        {
            var entities = message.Entities ?? new MessageEntity[0];
            var entityValues = message.EntityValues?.ToArray() ?? new string[0];
            var firstMessageEntity = entities.FirstOrDefault();

            var hasOneMessageEntity = entities.Length == 1 && entityValues.Length == 1;
            var firstEntityIsCommand = firstMessageEntity?.Offset == 0 && firstMessageEntity?.Type == MessageEntityType.BotCommand;

            return hasOneMessageEntity && firstEntityIsCommand;
        }
    }
}