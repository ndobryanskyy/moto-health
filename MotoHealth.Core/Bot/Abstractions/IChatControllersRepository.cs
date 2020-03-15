using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatControllersRepository
    {
        ValueTask<IChatController?> GetForChatAsync(long chatId, CancellationToken cancellationToken);

        Task AddAsync(IChatController controller, CancellationToken cancellationToken);
    }
}