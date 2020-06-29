using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public sealed class BannedUsersChatUpdateHandler : ChatUpdateHandlerBase
    {
        private static readonly IMessage HateMessage = MessageFactory.CreateTextMessage()
            .WithPlainText("https://youtu.be/h0ztJzMZbzM");

        private readonly IBotTelemetryService _telemetryService;

        public BannedUsersChatUpdateHandler(IBotTelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        protected override bool SkipGroupUpdates => true;

        protected override bool SkipHandledUpdates => true;

        protected override async Task OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            var state = await context.GetStagingStateAsync(cancellationToken);

            if (state.UserBanned)
            {
                await context.SendMessageAsync(HateMessage, cancellationToken);

                logger.LogDebug("Got a message from a banned chat");
                _telemetryService.OnMessageFromBannedChat();

                context.IsUpdateHandled = true;
            }
        }
    }
}