namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ITelegramUser
    {
        long Id { get; }

        string? Username { get; }

        string FirstName { get; }

        string? LastName { get; }

        string? LanguageCode { get; }
    }
}