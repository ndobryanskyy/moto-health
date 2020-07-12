using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ITelegramChat
    {
        long Id { get; }

        ChatType Type { get; }

        ITelegramGroup? Group { get; }
    }
}