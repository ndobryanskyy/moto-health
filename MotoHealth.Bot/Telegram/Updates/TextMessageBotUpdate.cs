namespace MotoHealth.Bot.Telegram.Updates
{
    internal sealed class TextMessageBotUpdate : MessageBotUpdateBase, ITextMessageBotUpdate
    {
        public TextMessageBotUpdate(
            int updateId,
            int messageId,
            IChatContext chat, 
            string text) 
            : base(updateId, messageId, chat)
        {
            Text = text;
        }

        public string Text { get; }
    }
}