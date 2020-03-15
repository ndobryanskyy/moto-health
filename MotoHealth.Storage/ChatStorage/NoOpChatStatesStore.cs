using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatStorage
{
    internal sealed class NoOpChatStatesStore : IChatStatesStore
    {
        public Task<IChatState?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IChatState?>(null);
        }

        public Task AddAsync(IChatState state, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}