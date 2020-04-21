using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatUpdateHandler
    {
        Task HandleUpdateAsync(IChatUpdateContext context, CancellationToken cancellationToken);
    }
}