using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class TelegramChat : ITelegramChat
    {
        public long Id { get; set; }

        public ChatType Type { get; set; }

        public ITelegramGroup? Group { get; set; }
    }
}