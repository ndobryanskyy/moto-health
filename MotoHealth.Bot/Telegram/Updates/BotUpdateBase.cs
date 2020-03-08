using System.Diagnostics;
using AutoMapper;
using Google.Protobuf;
using MotoHealth.BotUpdates;

namespace MotoHealth.Bot.Telegram.Updates
{
    internal abstract class BotUpdateBase : IBotUpdate
    {
        protected BotUpdateBase(int updateId)
        {
            UpdateId = updateId;
        }

        public int UpdateId { get; }

        public abstract IChatContext Chat { get; }

        public byte[] Serialize(IMapper mapper)
        {
            var botUpdate = new BotUpdateDto();

            SetActualBotUpdate(botUpdate, mapper);

            Debug.Assert(botUpdate.PayloadCase != BotUpdateDto.PayloadOneofCase.None);

            return botUpdate.ToByteArray();
        }

        protected abstract void SetActualBotUpdate(BotUpdateDto updateDto, IMapper mapper);
    }
}