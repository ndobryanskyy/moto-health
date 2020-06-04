using System.Threading;
using System.Threading.Tasks;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatSubscriptionsService
    {
        Task SubscribeChatToTopicAsync(long chatId, string topic, CancellationToken cancellationToken);

        Task UnsubscribeChatFromTopicAsync(long chatId, string topic, CancellationToken cancellationToken);
    }
}