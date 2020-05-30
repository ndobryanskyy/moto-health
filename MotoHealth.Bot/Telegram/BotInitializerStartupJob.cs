using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Telegram
{
    internal sealed class BotInitializerStartupJob : IHostedService
    {
        private readonly ILogger<BotInitializerStartupJob> _logger;
        private readonly IMapper _mapper;
        private readonly ITelegramClient _telegramClient;
        private readonly IBotCommandsRegistry _commandsRegistry;

        public BotInitializerStartupJob(
            ILogger<BotInitializerStartupJob> logger, 
            IMapper mapper, 
            ITelegramClient telegramClient,
            IBotCommandsRegistry commandsRegistry)
        {
            _logger = logger;
            _mapper = mapper;
            _telegramClient = telegramClient;
            _commandsRegistry = commandsRegistry;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot initialization");

            var commands = _mapper.Map<BotCommand[]>(_commandsRegistry.PublicCommands);
            
            var request = new SetMyCommandsRequest(commands);
            await _telegramClient.SetBotCommandsAsync(request, cancellationToken);

            _logger.LogInformation("Finished bot initialization");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }
    }
}