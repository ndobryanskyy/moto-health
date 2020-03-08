using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.ServiceBus;

namespace MotoHealth.Bot
{
    internal sealed class MessagesHandlerBackgroundService : BackgroundService
    {
        private readonly IQueueClient _queueClient;
        private readonly ILogger<MessagesHandlerBackgroundService> _logger;

        public MessagesHandlerBackgroundService(
            ILogger<MessagesHandlerBackgroundService> logger,
            IMessagesQueueReceiverClientProvider clientProvider)
        {
            _logger = logger;
            _queueClient = clientProvider.Client;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var handlerOptions = new SessionHandlerOptions(ExceptionReceivedHandler)
            {
                AutoComplete = false,
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
            //var body = JsonSerializer.Deserialize<TestBody>(message.Body);

            //Performance.FinishMeasuring(session.SessionId);

            //_logger.LogInformation($"Handling session: {session.SessionId}");

            //await Task.Delay(3000, cancellationToken);

            //await session.CompleteAsync(message.SystemProperties.LockToken);

            //_logger.LogInformation($"Processed: {body}");
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Exception while processing updates");

            return Task.CompletedTask;
        }
    }
}