using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Messages;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotUpdateContext
    {
        IBotUpdate Update { get; }

        Task SendMessageAsync(IMessage message, CancellationToken cancellationToken);
    }
}