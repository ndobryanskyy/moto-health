using System;
using AutoMapper;
using Google.Protobuf;
using Microsoft.Azure.ServiceBus;
using MotoHealth.BotUpdates;
using MotoHealth.Core.Bot.Updates;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Infrastructure.UpdatesQueue
{
    internal interface IBotUpdatesSerializer
    {
        byte[] SerializeToMessageBody(IBotUpdate botUpdate);

        IBotUpdate DeserializeFromMessage(Message message);
    }

    internal sealed class BotUpdatesSerializer : IBotUpdatesSerializer
    {
        private readonly IMapper _mapper;

        public BotUpdatesSerializer(IMapper mapper)
        {
            _mapper = mapper;
        }

        public byte[] SerializeToMessageBody(IBotUpdate botUpdate)
        {
            var mapped = _mapper.Map<BotUpdateDto>(botUpdate);
            
            return mapped.ToByteArray();
        }

        public IBotUpdate DeserializeFromMessage(Message message)
        {
            var parsed = BotUpdateDto.Parser.ParseFrom(message.Body);

            return parsed.PayloadCase switch
            {
                BotUpdateDto.PayloadOneofCase.TextMessage => _mapper.Map<TextMessageBotUpdate>(parsed.TextMessage),
                BotUpdateDto.PayloadOneofCase.Command => _mapper.Map<CommandBotUpdate>(parsed.Command),
                _ => throw new NotImplementedException("Incorrectly deserialized message should be sent to the dead letter queue")
            };
        }
    }
}