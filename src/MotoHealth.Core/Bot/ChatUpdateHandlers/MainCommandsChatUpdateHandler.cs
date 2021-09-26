using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Commands;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public sealed class MainCommandsChatUpdateHandler : ChatUpdateHandlerBase
    {
        private readonly IEnumerable<IBotCommand> _commands;

        public MainCommandsChatUpdateHandler(IEnumerable<IBotCommand> commands)
        {
            _commands = commands;
        }

        protected override bool SkipGroupUpdates => false;

        protected override bool SkipHandledUpdates => true;

        protected override async ValueTask OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            foreach (var command in _commands)
            {
                var executed = await command.TryExecuteAsync(context, cancellationToken);

                if (executed)
                {
                    context.IsUpdateHandled = true;

                    return;
                }
            }
        }
    }
}