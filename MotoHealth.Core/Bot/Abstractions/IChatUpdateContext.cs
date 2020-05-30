using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatUpdateContext
    {
        IChatUpdate Update { get; }

        Task SendMessageAsync(IMessage message, CancellationToken cancellationToken);
    }
}