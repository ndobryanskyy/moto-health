using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot.Commands
{
    public interface IBotCommand
    {
        ValueTask<bool> TryExecuteAsync(
            IChatUpdateContext chatUpdateContext,
            CancellationToken cancellationToken);
    }
}