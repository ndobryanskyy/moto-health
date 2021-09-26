namespace MotoHealth.Core.Bot.Updates.Abstractions
{
    public interface ITelegramContact
    {
        long UserId { get; }

        string PhoneNumber { get; }

        string? FirstName { get; }

        string? LastName { get; }
    }
}