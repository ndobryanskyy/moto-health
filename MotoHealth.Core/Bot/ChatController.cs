using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot
{
    internal sealed class ChatController : IChatController
    {
        public ChatController(long chatId, IChatState state)
        {
            ChatId = chatId;
            State = state;
        }

        private long ChatId { get; }

        private IChatState State { get; }

        public async Task HandleUpdateAsync(IBotUpdateContext context, CancellationToken cancellationToken)
        {
            if (context.Update is ITextMessageBotUpdate textMessage)
            {
                await context.SendTextMessageAsync(textMessage.Text, cancellationToken);
            }
        }
    }
}