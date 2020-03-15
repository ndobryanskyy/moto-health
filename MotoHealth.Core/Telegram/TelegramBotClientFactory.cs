﻿using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;
using Telegram.Bot;

namespace MotoHealth.Core.Telegram
{
    internal sealed class TelegramBotClientFactory : ITelegramBotClientFactory
    {
        private readonly string _botToken;

        public TelegramBotClientFactory(IOptions<TelegramOptions> options)
        {
            _botToken = options.Value.BotToken;
        }

        public ITelegramBotClient CreateClient() => new TelegramBotClient(_botToken);
    }
}