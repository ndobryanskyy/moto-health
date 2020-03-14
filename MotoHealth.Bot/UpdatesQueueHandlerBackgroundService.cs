using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Messages;
using MotoHealth.Bot.ServiceBus;

namespace MotoHealth.Bot
{
    internal sealed class UpdatesQueueHandlerBackgroundService : BackgroundService
    {
        private readonly IQueueClient _queueClient;
        private readonly ILogger<UpdatesQueueHandlerBackgroundService> _logger;
        private readonly IBotUpdateSerializer _serializer;
        private readonly IBotContextFactory _botContextFactory;
        private readonly IBotsRepository _botsRepository;

        public UpdatesQueueHandlerBackgroundService(
            ILogger<UpdatesQueueHandlerBackgroundService> logger,
            IConfiguration configuration,
            IQueueClientsFactory clientsFactory,
            IBotUpdateSerializer serializer,
            IBotContextFactory botContextFactory,
            IBotsRepository botsRepository)
        {
            _logger = logger;
            _serializer = serializer;
            _botContextFactory = botContextFactory;
            _botsRepository = botsRepository;

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

            _logger.LogDebug($"Deserialized update {botUpdate.UpdateId} successfully");

            var bot = await _botsRepository.GetBotForChatAsync(botUpdate.Chat.Id, cancellationToken);
            var context = _botContextFactory.CreateForUpdate(bot, botUpdate);

            _logger.LogDebug($"Bot started handling update: {context.Update.UpdateId} in chat: {context.Update.Chat.Id}");

            await bot.HandleUpdateAsync(context, cancellationToken);

            _logger.LogDebug($"Bot finished handling update: {context.Update.UpdateId} in chat: {context.Update.Chat.Id}");

            await session.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Exception while polling queue");

            return Task.CompletedTask;
        }
    }
}