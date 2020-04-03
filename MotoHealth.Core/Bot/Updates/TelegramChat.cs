using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class TelegramChat : ITelegramChat
    {
        public TelegramChat(
            long id,
            TelegramUser from,
            TelegramGroup? group)
        {
            Id = id;
            From = from;
            Group = group;
        }

        public long Id { get; }

        public ITelegramUser From { get; }

        public ITelegramGroup? Group { get; }
    }
}