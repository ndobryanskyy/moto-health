using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatStatesStore
    {
        Task<IChatState?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken);

        Task AddAsync(IChatState state, CancellationToken cancellationToken);
    }
}