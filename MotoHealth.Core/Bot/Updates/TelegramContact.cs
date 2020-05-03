using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class TelegramContact : ITelegramContact
    {
        public int UserId { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}