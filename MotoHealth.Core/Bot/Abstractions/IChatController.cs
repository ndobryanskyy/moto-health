using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatController
    {
        long ChatId { get; }

        Task HandleUpdateAsync(IBotUpdateContext context, CancellationToken cancellationToken);
    }
}