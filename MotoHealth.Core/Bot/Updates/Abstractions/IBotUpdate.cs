namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IBotUpdate
    {
        int UpdateId { get; }

        ITelegramUser Sender { get; }

        ITelegramChat Chat { get; }
    }
}