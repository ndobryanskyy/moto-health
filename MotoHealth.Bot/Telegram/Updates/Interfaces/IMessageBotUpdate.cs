using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram.Updates
{
    public interface IMessageBotUpdate : IBotUpdate
    {
        Message Message { get; }
    }
}