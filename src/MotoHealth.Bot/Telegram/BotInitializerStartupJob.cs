using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Bot.Telegram
{
    internal sealed class BotInitializerStartupJob : IHostedService
    {
        private readonly IBotInitializer _botInitializer;

        public BotInitializerStartupJob(IBotInitializer botInitializer)
        {
            _botInitializer = botInitializer;
        }

        public async Task StartAsync(CancellationToken cancellationToken) 
            => await _botInitializer.InitializeAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }
    }
}