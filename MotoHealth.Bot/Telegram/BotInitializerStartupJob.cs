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
        private readonly ITelegramBotClient _botClient;
        private readonly IBotCommandsRegistry _commandsRegistry;

        public BotInitializerStartupJob(
            ILogger<BotInitializerStartupJob> logger, 
            IMapper mapper, 
            ITelegramBotClient botClient,
            IBotCommandsRegistry commandsRegistry)
        {
            _logger = logger;
            _mapper = mapper;
            _botClient = botClient;
            _commandsRegistry = commandsRegistry;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot initialization");

            var commands = _mapper.Map<BotCommand[]>(_commandsRegistry.PublicCommands);

            await _botClient.SetMyCommandsAsync(commands, cancellationToken);

            _logger.LogInformation("Finished bot initialization");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }
    }
}