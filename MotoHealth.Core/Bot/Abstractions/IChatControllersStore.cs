using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatControllersStore
    {
        Task<IChatController?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken);

        Task SaveControllerAsync(IChatController controller, CancellationToken cancellationToken);
    }
}