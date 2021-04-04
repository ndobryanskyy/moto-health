using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class InMemoryChatsDoorman : IChatsDoorman
    {
        private readonly ILogger<InMemoryChatsDoorman> _logger;

        private readonly ConcurrentDictionary<long, ChatLock> _locks = new ConcurrentDictionary<long, ChatLock>();

        public InMemoryChatsDoorman(ILogger<InMemoryChatsDoorman> logger)
        {
            _logger = logger;
        }

        public bool TryLockChat(long chatId, [NotNullWhen(true)] out IDisposable? chatLock)
        {
            chatLock = null;

            var newLock = new ChatLock(chatId);
            var added = _locks.TryAdd(chatId, newLock);

            if (!added)
            {
                _logger.LogInformation($"Chat {chatId} already locked!");
                
                return false;
            }
         
            _logger.LogDebug($"Chat {chatId} locked");

            newLock.Released += OnChatLockReleased;
            chatLock = newLock;
            
            return true;
        }

        private void OnChatLockReleased(object sender, EventArgs e)
        {
            if (sender is ChatLock chatLock)
            {
                if (_locks.TryRemove(chatLock.ChatId, out _))
                {
                    _logger.LogDebug($"Lock for chat {chatLock.ChatId} released");
                }
            }
            else
            {
                throw new ArgumentException(nameof(sender));
            }
        }

        private class ChatLock : IDisposable
        {
            private bool _released;

            public ChatLock(long chatId)
            {
                ChatId = chatId;
            }

            public long ChatId { get; }

            public event EventHandler? Released;

            public void Dispose()
            {
                if (_released)
                {
                    return;
                }
                
                Released?.Invoke(this, EventArgs.Empty);

                _released = true;
            }
        }
    }
}