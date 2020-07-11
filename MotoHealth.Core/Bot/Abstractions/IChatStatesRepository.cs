using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatStatesRepository
    {
        Task<IChatState> CreateForChatAsync(long chatId, CancellationToken cancellationToken);

        ValueTask<IChatState?> GetForChatAsync(long chatId, CancellationToken cancellationToken);

        Task UpdateAsync(IChatState state, CancellationToken cancellationToken);
    }
}