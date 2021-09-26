using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class TelegramUser : ITelegramUser
    {
        public long Id { get; set; }

        public string? Username { get; set; }

        public string FirstName { get; set; } = default!;

        public string? LastName { get; set; }

        public string? LanguageCode { get; set; }
    }
}