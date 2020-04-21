﻿using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.ChatStorage.Entities;

namespace MotoHealth.Infrastructure.ChatStorage
{
    internal sealed class DefaultChatStateFactory : IDefaultChatStateFactory
    {
        public IChatState CreateDefaultState(long chatId)
            => new ChatState
            {
                AssociatedChatId = chatId
            };

    }
}