using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Commands;
using MotoHealth.Telegram;
using Telegram.Bot.Requests;

namespace MotoHealth.Bot.Telegram
{
    internal sealed class BotInitializer : IBotInitializer
    {
        private readonly ILogger<BotInitializer> _logger;
        private readonly ITelegramClient _telegramClient;
        private readonly IPublicCommandsProvider _publicCommandsProvider;
        
        private readonly TelegramOptions _telegramOptions;

        public BotInitializer(
            ILogger<BotInitializer> logger,
            IOptions<TelegramOptions> telegramOptions,
            ITelegramClient telegramClient,
            IPublicCommandsProvider publicCommandsProvider)
        {
            _logger = logger;
            _telegramClient = telegramClient;
            _publicCommandsProvider = publicCommandsProvider;

            _telegramOptions = telegramOptions.Value;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot initialization");

            try
            {
                await SetWebhookAsync(cancellationToken);
                await SetCommandsAsync(cancellationToken);

                _logger.LogInformation("Finished bot initialization");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to initialize bot");

                throw;
            }
        }

        private async Task SetWebhookAsync(CancellationToken cancellationToken)
        {
            var webhookOptions = _telegramOptions.Webhook;

            var webhookUrl = GetWebhookUrl();
            var maxConnections = webhookOptions.MaxConnections;
            var allowedUpdates = webhookOptions.AllowedUpdates;

            // TODO: sanitize before logging
            _logger.LogDebug($"Setting webhook to {webhookUrl}");

            _logger.LogInformation(
                "Webhook settings:\n" +
                $"Allowed updates: {string.Join(", ", allowedUpdates)}\n" +
                $"Max connections: {maxConnections}");

            var webhookRequest = new SetWebhookRequest(webhookUrl, null)
            {
                MaxConnections = _telegramOptions.Webhook.MaxConnections,
                AllowedUpdates = _telegramOptions.Webhook.AllowedUpdates
            };

            await _telegramClient.SetWebhookAsync(webhookRequest, cancellationToken);

            _logger.LogInformation("Successfully set webhook");
        }

        private async Task SetCommandsAsync(CancellationToken cancellationToken)
        {
            var publicCommands = _publicCommandsProvider.Commands;

            _logger.LogDebug($"Discovered {publicCommands.Length} public commands");

            var commandsRequest = new SetMyCommandsRequest(publicCommands);
            await _telegramClient.SetBotCommandsAsync(commandsRequest, cancellationToken);

            _logger.LogInformation("Successfully set commands");
        }

        private string GetWebhookUrl()
        {
            var clientOptions = _telegramOptions.Client;

            var baseUrl = _telegramOptions.Webhook.BaseUrl.TrimEnd('/');
            var url = $"{baseUrl}{Constants.Telegram.WebhookPath}";

            var queryParams = new Dictionary<string, string>
            {
                { Constants.Telegram.BotIdQueryParamName, clientOptions.BotId },
                { Constants.Telegram.BotSecretQueryParamName, clientOptions.BotSecret }
            };

            var webhookUrl = QueryHelpers.AddQueryString(url, queryParams);

            return webhookUrl;
        }
    }
}