using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotUpdatesQueue
    {
        Task EnqueueUpdateAsync(IBotUpdate botUpdate, CancellationToken cancellationToken);
    }
}