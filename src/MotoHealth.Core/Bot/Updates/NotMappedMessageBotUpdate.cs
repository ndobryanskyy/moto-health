using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class NotMappedMessageBotUpdate : MessageBotUpdateBase, INotMappedMessageBotUpdate
    {
        public Update OriginalUpdate { get; set; } = default!;

        public Message OriginalMessage { get; set; } = default!;
    }
}