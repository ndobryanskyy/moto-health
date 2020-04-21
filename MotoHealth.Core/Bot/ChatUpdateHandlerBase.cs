using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot
{
    public abstract class ChatUpdateHandlerBase : IChatUpdateHandler
    {
        private Chat? _chat;

        public async Task HandleUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            await OnUpdateAsync(context, cancellationToken);
        }

        internal Chat Chat
        {
            get => _chat ?? throw new InvalidOperationException();
            set => _chat = value;
        }

        protected async ValueTask<IChatState> GetStateAsync(CancellationToken cancellationToken)
            => await Chat.GetStateAsync(cancellationToken);

        protected async Task UpdateStateAsync(IChatState state, CancellationToken cancellationToken)
            => await Chat.UpdateStateAsync(state, cancellationToken);

        protected bool TryLockChat([NotNullWhen(true)] out IDisposable? chatLock)
            => Chat.TryLock(out chatLock);

        protected abstract Task OnUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken);
    }
}