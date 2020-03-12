namespace MotoHealth.Bot.Telegram.Updates
{
    public interface ITextMessageBotUpdate : IMessageBotUpdate
    {
        public string Text { get; }
    }
}