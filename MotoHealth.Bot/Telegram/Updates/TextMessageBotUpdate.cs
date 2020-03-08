using AutoMapper;
using MotoHealth.BotUpdates;

namespace MotoHealth.Bot.Telegram.Updates
{
    internal sealed class TextMessageBotUpdate : MessageBotUpdate, ITextMessageBotUpdate
    {
        public TextMessageBotUpdate(
            int updateId,
            IChatContext chat, 
            string text) 
            : base(updateId, chat)
        {
            Text = text;
        }

        public string Text { get; }

        protected override void SetActualBotUpdate(BotUpdateDto update, IMapper mapper)
        {
            update.TextMessage = mapper.Map<TextMessageBotUpdateDto>(this);
        }
    }
}