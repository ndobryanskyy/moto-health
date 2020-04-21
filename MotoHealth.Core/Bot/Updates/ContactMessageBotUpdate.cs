using System;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public sealed class ContactMessageBotUpdate : MessageBotUpdateBase, IContactMessageBotUpdate
    {
        public TelegramContact? Contact { get; internal set; }

        ITelegramContact IContactMessageBotUpdate.Contact => Contact ?? throw new InvalidOperationException();
    }
}