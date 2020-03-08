namespace MotoHealth.Bot.Telegram.Updates
{
    public interface ITextMessageBotUpdate : IBotUpdate
    {
        public string Text { get; }
    }
}