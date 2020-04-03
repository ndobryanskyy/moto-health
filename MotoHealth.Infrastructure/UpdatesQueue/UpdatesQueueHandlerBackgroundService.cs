using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.ServiceBus;

namespace MotoHealth.Infrastructure.UpdatesQueue
{
    internal sealed class UpdatesQueueHandlerBackgroundService : BackgroundService
    {
        private readonly IQueueClient _queueClient;
        private readonly ILogger<UpdatesQueueHandlerBackgroundService> _logger;
        private readonly IBotUpdatesSerializer _updatesSerializer;
        private readonly IServiceProvider _services;

        public UpdatesQueueHandlerBackgroundService(
            ILogger<UpdatesQueueHandlerBackgroundService> logger,
            IOptions<UpdatesQueueOptions> updatesQueueOptions,
            IServiceBusClientsFactory clientsFactory,
            IBotUpdatesSerializer updatesSerializer,
            IServiceProvider services)
        {
            _logger = logger;
            _updatesSerializer = updatesSerializer;
            _services = services;

            var connectionString = updatesQueueOptions.Value.ConnectionString;
            var builder = new ServiceBusConnectionStringBuilder(connectionString);

            _queueClient = clientsFactory.CreateSessionHandlingClient(builder);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var handlerOptions = new SessionHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MaxConcurrentSessions = 1
            };

            _queueClient.RegisterSessionHandler(HandleUpdatesAsync, handlerOptions);

            _logger.LogInformation("Successfully registered session handler for queue");

            var completionSource = new TaskCompletionSource<object>();

            stoppingToken.Register(() =>
            {
                _queueClient.CloseAsync()
                    .ContinueWith(t =>
                    {
                        completionSource.TrySetCanceled(stoppingToken);
                    });
            });

            return completionSource.Task;
        }

        private async Task HandleUpdatesAsync(IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            using var servicesScope = _services.CreateScope();

            var botUpdate = _updatesSerializer.DeserializeFromMessage(message);

            _logger.LogDebug($"Deserialized update {botUpdate.UpdateId} successfully");

            var handler = servicesScope.ServiceProvider.GetRequiredService<IBotUpdateHandler>();
            await handler.HandleBotUpdateAsync(botUpdate, cancellationToken);

            await session.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Exception while polling queue");

            return Task.CompletedTask;
        }
    }
}