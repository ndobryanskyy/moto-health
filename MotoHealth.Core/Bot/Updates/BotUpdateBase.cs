using System;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    public abstract class BotUpdateBase : IBotUpdate
    {
        public int UpdateId { get; internal set; }

        public TelegramUser? Sender { get; internal set; }

        public TelegramChat? Chat { get; internal set; }

        ITelegramUser IBotUpdate.Sender => Sender ?? throw new InvalidOperationException();

        ITelegramChat IBotUpdate.Chat => Chat ?? throw new InvalidOperationException();
    }
}