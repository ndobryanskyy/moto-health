namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface IChatContext
    {
        long Id { get; }

        bool IsGroup { get; }

        string? Username { get; }

        string? Title { get; }
    }
}