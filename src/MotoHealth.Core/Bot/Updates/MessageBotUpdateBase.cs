using System.Diagnostics.CodeAnalysis;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal abstract class MessageBotUpdateBase : BotUpdateBase, IMessageBotUpdate
    {
        public int MessageId { get; set; }

        public long ChatId => Chat.Id;

        [NotNull] public TelegramUser? Sender { get; set; }

        [NotNull] public TelegramChat? Chat { get; set; }

        ITelegramUser IHasSender.Sender => Sender;

        ITelegramChat IBelongsToChat.Chat => Chat;
    }
}