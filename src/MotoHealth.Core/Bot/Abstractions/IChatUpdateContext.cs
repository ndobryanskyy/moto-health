using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatUpdateContext
    {
        long ChatId { get; }

        IChatUpdate Update { get; }

        bool IsUpdateHandled { get; set; }

        ValueTask<IChatState> GetStagingStateAsync(CancellationToken cancellationToken);

        Task SendMessageAsync(IMessage message, CancellationToken cancellationToken);
    }
}