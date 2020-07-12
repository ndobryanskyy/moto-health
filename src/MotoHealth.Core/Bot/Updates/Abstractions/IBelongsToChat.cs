namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IBelongsToChat
    {
        ITelegramChat Chat { get; }
    }
}