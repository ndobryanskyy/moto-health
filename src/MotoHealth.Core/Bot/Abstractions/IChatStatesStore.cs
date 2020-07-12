using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatStatesStore
    {
        Task<IChatState> CreateAsync(long chatId, CancellationToken cancellationToken);

        Task<IChatState?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken);

        Task UpdateAsync(IChatState state, CancellationToken cancellationToken);
    }
}