namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ITelegramUser
    {
        int Id { get; }

        string? Username { get; }

        string? FirstName { get; }

        string? LastName { get; }

        string? LanguageCode { get; }
    }
}