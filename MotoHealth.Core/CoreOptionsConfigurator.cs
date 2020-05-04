using System;
using MotoHealth.Telegram;

namespace MotoHealth.Core
{
    public sealed class CoreOptionsConfigurator
    {
        public Action<TelegramOptions>? ConfigureTelegram { get; set; }
    }
}