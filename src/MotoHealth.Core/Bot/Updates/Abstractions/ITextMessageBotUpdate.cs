namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ITextMessageBotUpdate : IMessageBotUpdate
    {
        public string Text { get; }
    }
}