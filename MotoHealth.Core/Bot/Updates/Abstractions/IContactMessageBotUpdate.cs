namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IContactMessageBotUpdate : IMessageBotUpdate
    {
        public ITelegramContact Contact { get; }
    }
}