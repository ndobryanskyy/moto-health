using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatControllersStorage
{
    internal sealed class NoOpChatControllersStore : IChatControllersStore
    {
        public Task<IChatController?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IChatController?>(null);
        }

        public Task SaveControllerAsync(IChatController controller, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}