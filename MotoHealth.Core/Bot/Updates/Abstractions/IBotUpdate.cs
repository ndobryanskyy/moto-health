namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IBotUpdate
    {
        int UpdateId { get; }

        IChatContext Chat { get; }
    }
}