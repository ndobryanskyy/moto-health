using Telegram.Bot.Types;

namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface INotMappedMessageBotUpdate : INotMappedBotUpdate
    {
        Message OriginalMessage { get; }
    }
}