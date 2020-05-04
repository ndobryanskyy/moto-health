﻿using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatUpdateContext : IChatUpdateContext
    {
        private readonly long _chatId;

        private readonly ITelegramBotClient _client;

        public ChatUpdateContext(IChatUpdate update, ITelegramBotClient client)
        {
            _client = client;

            Update = update;

            _chatId = Update.Chat.Id;
        }

        public IChatUpdate Update { get; }

        public async Task SendMessageAsync(IMessage message, CancellationToken cancellationToken)
        {
            await message.SendAsync(_chatId, _client, cancellationToken);
        }

        public async Task SetChatActionAsync(ChatAction action, CancellationToken cancellationToken)
        {
            await _client.SendChatActionAsync(_chatId, ChatAction.Typing, cancellationToken);
        }
    }
}