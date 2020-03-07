using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram.Updates
{
    internal sealed class TextMessageBotUpdate : MessageBotUpdate, ITextMessageBotUpdate
    {
        public TextMessageBotUpdate(Update update, string text) 
            : base(update)
        {
            Text = text;
        }

        public string Text { get; }
    }
}