using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace MotoHealth.Telegram
{
    public interface ITelegramClient
    {
        Task<User> GetMeAsync(CancellationToken cancellationToken);

        Task SendTextMessageAsync(
            SendMessageRequest request,
            CancellationToken cancellationToken);

        Task SendVenueMessageAsync(
            SendVenueRequest request,
            CancellationToken cancellationToken);

        Task SetBotCommandsAsync(
            SetMyCommandsRequest request,
            CancellationToken cancellationToken);

        Task SetWebhookAsync(
            SetWebhookRequest request,
            CancellationToken cancellationToken);
    }
}