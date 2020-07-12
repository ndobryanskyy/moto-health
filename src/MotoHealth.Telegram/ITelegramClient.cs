using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests;

namespace MotoHealth.Telegram
{
    public interface ITelegramClient
    {
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