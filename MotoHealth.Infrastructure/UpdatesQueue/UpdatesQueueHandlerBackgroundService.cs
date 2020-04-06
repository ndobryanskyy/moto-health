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
        
        private readonly SessionHandlerOptions _handlerOptions;

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

            var options = updatesQueueOptions.Value;

            var connectionString = options.ConnectionString;
            var builder = new ServiceBusConnectionStringBuilder(connectionString);

            _queueClient = clientsFactory.CreateSessionHandlingClient(builder);

            _handlerOptions = new SessionHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
                MessageWaitTimeout = TimeSpan.FromSeconds(options.MessageWaitTimeoutInSeconds),
                MaxConcurrentSessions = options.MaxConcurrentHandlers
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _queueClient.RegisterSessionHandler(HandleUpdatesAsync, _handlerOptions);

            _logger.LogInformation(
                $"Successfully registered session handler for queue with {_handlerOptions.MaxConcurrentSessions} max concurrent sessions " + 
                $"and timeout of {_handlerOptions.MessageWaitTimeout.TotalSeconds} seconds"
            );

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