using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public abstract class ChatUpdateHandlerBase
    {
        public async Task HandleUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            if (SkipHandledUpdates && context.IsUpdateHandled)
            {
                logger.LogDebug("Skipping already handled update");
            }
            else
            {
                if (SkipGroupUpdates && context.Update.Chat.Type != ChatType.Private)
                {
                    logger.LogDebug("Skipping group update");
                }
                else
                {
                    logger.LogDebug("Start Handling");

                    await OnUpdateAsync(context, logger, cancellationToken);

                    logger.LogDebug("Finished Handling");
                }
            }
        }

        protected abstract bool SkipGroupUpdates { get; }

        protected abstract bool SkipHandledUpdates { get; }

        protected abstract Task OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken);
    }
}