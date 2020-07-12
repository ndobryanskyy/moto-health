using System.Diagnostics.CodeAnalysis;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class ContactMessageBotUpdate : MessageBotUpdateBase, IContactMessageBotUpdate
    {
        [NotNull] public TelegramContact? Contact { get; set; }

        ITelegramContact IContactMessageBotUpdate.Contact => Contact;
    }
}