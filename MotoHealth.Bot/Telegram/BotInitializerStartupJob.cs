using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram
{
    internal sealed class BotInitializerStartupJob : IHostedService
    {
        private readonly ILogger<BotInitializerStartupJob> _logger;
        private readonly IMapper _mapper;
        private readonly IBotCommandsRegistry _commandsRegistry;
        private readonly ITelegramBotClient _telegramClient;

        public BotInitializerStartupJob(
            ILogger<BotInitializerStartupJob> logger, 
            IMapper mapper, 
            ITelegramBotClientFactory clientFactory,
            IBotCommandsRegistry commandsRegistry)
        {
            _logger = logger;
            _mapper = mapper;
            _commandsRegistry = commandsRegistry;
            _telegramClient = clientFactory.CreateClient();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot initialization");

            var commands = _mapper.Map<BotCommand[]>(_commandsRegistry.PublicCommands);

            await _telegramClient.SetMyCommandsAsync(commands, cancellationToken);

            _logger.LogInformation("Finished bot initialization");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }
    }
}