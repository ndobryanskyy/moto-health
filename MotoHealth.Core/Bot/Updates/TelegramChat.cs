using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class TelegramChat : ITelegramChat
    {
        public long Id { get; internal set; }

        public ChatType Type { get; internal set; }

        public ITelegramGroup? Group { get; internal set; }
    }
}