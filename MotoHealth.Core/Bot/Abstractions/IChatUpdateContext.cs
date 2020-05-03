﻿using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatUpdateContext
    {
        IChatUpdate Update { get; }

        Task SendMessageAsync(IMessage message, CancellationToken cancellationToken);

        Task SetChatActionAsync(ChatAction action, CancellationToken cancellationToken);
    }
}