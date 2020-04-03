namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ITelegramChat
    {
        long Id { get; }

        ITelegramUser From { get; }

        ITelegramGroup? Group { get; }
    }
}