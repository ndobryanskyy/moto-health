namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IBotUpdate
    {
        int UpdateId { get; }

        ITelegramChat Chat { get; }
    }
}