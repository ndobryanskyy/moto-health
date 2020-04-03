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

            return _mapper.Map<IBotUpdate>(parsed);
        }
    }
}