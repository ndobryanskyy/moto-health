using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using Telegram.Bot;

namespace MotoHealth.Core.Bot
{
    internal sealed class BotUpdateContext : IBotUpdateContext
    {
        private readonly ITelegramBotClient _client;

        public BotUpdateContext(IBotUpdate update, ITelegramBotClient client)
        {
            _client = client;

            Update = update;
        }

        public IBotUpdate Update { get; }

        public async Task SendTextMessageAsync(string text, CancellationToken cancellationToken)
        {
            await _client.SendTextMessageAsync(Update.Chat.Id, text, cancellationToken: cancellationToken);
        }
    }
}