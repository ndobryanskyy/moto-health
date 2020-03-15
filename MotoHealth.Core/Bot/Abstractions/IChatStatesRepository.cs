using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatStatesRepository
    {
        ValueTask<IChatState?> GetForChatAsync(long chatId, CancellationToken cancellationToken);

        Task AddAsync(IChatState state, CancellationToken cancellationToken);
    }
}