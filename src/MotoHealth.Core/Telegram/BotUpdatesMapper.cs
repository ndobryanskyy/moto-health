using System;
using System.Linq;
using AutoMapper;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Telegram
{
    public interface IBotUpdatesMapper
    {
        IBotUpdate MapTelegramUpdate(Update update);
    }

    internal sealed class BotUpdatesMapper : IBotUpdatesMapper
    {
        private readonly IMapper _mapper;

        public BotUpdatesMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IBotUpdate MapTelegramUpdate(Update update)
        {
            return update switch
            {
                { Type: UpdateType.Message, Message: Message _ } => MapMessageBotUpdate(update),
                _ => _mapper.Map<NotMappedBotUpdate>(update)
            };
        }

        private IMessageBotUpdate MapMessageBotUpdate(Update update)
        {
            var message = update.Message ?? throw new ArgumentException(nameof(update));

            return message switch
            {
                { Type: MessageType.Location } => _mapper.Map<LocationMessageBotUpdate>(update),
                { Type: MessageType.Contact, Contact: Contact _ } => _mapper.Map<ContactMessageBotUpdate>(update),
                { Type: MessageType.Text } when HasOnlyOneCommandEntity(message) => _mapper.Map<CommandMessageBotUpdate>(update),
                { Type: MessageType.Text } => _mapper.Map<TextMessageBotUpdate>(update),
                _ => _mapper.Map<NotMappedMessageBotUpdate>(update)
            };
        }

        private static bool HasOnlyOneCommandEntity(Message message)
        {
            var commandEntities = message.Entities?
                .Where(x => x.Type == MessageEntityType.BotCommand)
                .ToArray();

            return commandEntities != null && 
                   commandEntities.Length == 1 &&
                   commandEntities.First().Offset == 0;
        }
    }
}