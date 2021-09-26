using System;
using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Commands
{
    public abstract class BotCommandBase : IBotCommand
    {
        private readonly CommandName _name;

        protected BotCommandBase(CommandName name)
        {
            _name = name;
        }

        public async ValueTask<bool> TryExecuteAsync(IChatUpdateContext updateContext, CancellationToken cancellationToken)
        {
            if (updateContext.Update is ICommandMessageBotUpdate commandMessage &&
                Matches(commandMessage))
            {
                await ExecuteAsync(updateContext, commandMessage, cancellationToken);
                return true;
            }

            return false;
        }

        protected virtual bool Matches(ICommandMessageBotUpdate commandMessage)
            => commandMessage.Command.Equals(_name, StringComparison.InvariantCultureIgnoreCase);

        protected abstract Task ExecuteAsync(IChatUpdateContext context, ICommandMessageBotUpdate command, CancellationToken cancellationToken);
    }
}