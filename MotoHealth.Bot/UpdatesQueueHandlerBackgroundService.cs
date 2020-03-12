using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Messages;
using MotoHealth.Bot.ServiceBus;
using MotoHealth.Bot.Telegram.Updates;

namespace MotoHealth.Bot
{
    internal sealed class UpdatesQueueHandlerBackgroundService : BackgroundService
    {
        private readonly IQueueClient _queueClient;
        private readonly ILogger<UpdatesQueueHandlerBackgroundService> _logger;
        private readonly IBotUpdateSerializer _serializer;

        public UpdatesQueueHandlerBackgroundService(
            ILogger<UpdatesQueueHandlerBackgroundService> logger,
            IConfiguration configuration,
            IQueueClientsFactory clientsFactory,
            IBotUpdateSerializer serializer)
        {
            _logger = logger;
            _serializer = serializer;

            var connectionString = configuration.GetConnectionString(Constants.UpdatesQueue.ConnectionStringName);
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
            var botUpdate = _serializer.DeserializeFromMessage(message);

            if (botUpdate is ITextMessageBotUpdate)
            {
                _logger.LogInformation("This is a text message");
            }

            await session.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Exception while polling queue");

            return Task.CompletedTask;
        }
    }
}