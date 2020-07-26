using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public abstract class ChatUpdateHandlerBase
    {
        public async ValueTask HandleUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            if (SkipHandledUpdates && context.IsUpdateHandled)
            {
                logger.LogTrace("Skipping already handled update");
            }
            else
            {
                if (SkipGroupUpdates && context.Update.Chat.Type != ChatType.Private)
                {
                    logger.LogTrace("Skipping group update");
                }
                else
                {
                    logger.LogTrace("Start Handling");

                    await OnUpdateAsync(context, logger, cancellationToken);

                    logger.LogTrace("Finished Handling");
                }
            }
        }

        protected abstract bool SkipGroupUpdates { get; }

        protected abstract bool SkipHandledUpdates { get; }

        protected abstract ValueTask OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken);
    }
}