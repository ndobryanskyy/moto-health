using System;
using MotoHealth.Core.Telegram;

namespace MotoHealth.Core
{
    public sealed class CoreOptionsConfigurator
    {
        public Action<TelegramOptions>? ConfigureTelegram { get; set; }
    }
}