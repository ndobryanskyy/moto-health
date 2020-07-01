using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly TelegramOptions _telegramOptions;

        public BotInitializerStartupJob(
            ILogger<BotInitializerStartupJob> logger,
            IOptions<TelegramOptions> telegramOptions,
            ITelegramClient telegramClient,
            IBotCommandsRegistry commandsRegistry,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _telegramClient = telegramClient;
            _commandsRegistry = commandsRegistry;
            _telegramOptions = telegramOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot initialization");

            await SetWebhookAsync(cancellationToken);
            await SetCommandsAsync(cancellationToken);

            _logger.LogInformation("Finished bot initialization");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Does nothing

            return Task.CompletedTask;
        }

        private async Task SetWebhookAsync(CancellationToken cancellationToken)
        {
            var clientOptions = _telegramOptions.Client;

            var baseUrl = _telegramOptions.Webhook.BaseUrl.TrimEnd('/');
            var url = $"{baseUrl}{Constants.Telegram.WebhookPath}";

            var queryParams = new Dictionary<string, string>
            {
                { Constants.Telegram.BotIdQueryParamName, clientOptions.BotId },
                { Constants.Telegram.BotSecretQueryParamName, clientOptions.BotSecret }
            };

            var webhookUrl =  QueryHelpers.AddQueryString(url, queryParams);

            var webHookRequest = new SetWebhookRequest(webhookUrl, null)
            {
                MaxConnections = _telegramOptions.Webhook.MaxConnections,
                AllowedUpdates = _telegramOptions.Webhook.AllowedUpdates
            };

            await _telegramClient.SetWebhookAsync(webHookRequest, cancellationToken);

            _logger.LogInformation("Successfully set webhook");
        }

        private async Task SetCommandsAsync(CancellationToken cancellationToken)
        {
            var commands = _mapper.Map<BotCommand[]>(_commandsRegistry.PublicCommands);

            var commandsRequest = new SetMyCommandsRequest(commands);
            await _telegramClient.SetBotCommandsAsync(commandsRequest, cancellationToken);

            _logger.LogInformation("Successfully set commands");
        }
    }
}