using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Bot.Telegram.Updates;
using Telegram.Bot;

namespace MotoHealth.Bot
{
    internal interface IBotController
    {
        ITelegramBotClient TelegramClient { get; }

        Task HandleUpdateAsync(IBotUpdateContext context, CancellationToken cancellationToken);
    }

    internal sealed class BotController : IBotController
    {
        public BotController(ITelegramBotClient botClient)
        {
            TelegramClient = botClient;
        }

        public ITelegramBotClient TelegramClient { get; }

        public async Task HandleUpdateAsync(IBotUpdateContext context, CancellationToken cancellationToken)
        {
            if (context.Update is ITextMessageBotUpdate textMessage)
            {
                await context.SendTextMessageAsync(textMessage.Text, cancellationToken);
            }
        }
    }
}