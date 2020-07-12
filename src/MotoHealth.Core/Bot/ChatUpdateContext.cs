using System;
using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Core.Extensions;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatUpdateContext : IChatUpdateContext
    {
        private readonly ITelegramClient _client;
        private readonly IChatStatesRepository _chatStatesRepository;

        private IChatState? _chatState;

        public ChatUpdateContext(
            IChatUpdate chatUpdate,
            ITelegramClient client,
            IChatStatesRepository chatStatesRepository)
        {
            _client = client;
            _chatStatesRepository = chatStatesRepository;

            Update = chatUpdate;
        }

        public long ChatId => Update.Chat.Id;

        public IChatUpdate Update { get; }

        public bool IsUpdateHandled { get; set; }

        public async ValueTask<IChatState> GetStagingStateAsync(CancellationToken cancellationToken)
        {
            if (_chatState == null)
            {
                var state = await _chatStatesRepository.GetForChatAsync(ChatId, cancellationToken) 
                            ?? throw new InvalidOperationException();
                
                _chatState = state.Clone();
            }

            return _chatState;
        }

        public async Task SendMessageAsync(IMessage message, CancellationToken cancellationToken) 
            => await message.SendAsync(ChatId, _client, cancellationToken);
    }
}