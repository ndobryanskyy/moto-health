using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Bot.Abstractions;
using MotoHealth.Bot.Telegram.Updates;
using Telegram.Bot;

namespace MotoHealth.Bot
{
    internal interface IBotUpdateContext : IConversationContext
    {
        IBotUpdate Update { get; }
    }

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