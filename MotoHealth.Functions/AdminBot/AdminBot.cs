using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Functions.AdminBot.Authorization;
using MotoHealth.Telegram.Extensions;
using MotoHealth.Telegram.Messages;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Functions.AdminBot
{
    public interface IAdminBot
    {
        Task HandleUpdateAsync(Update update);
    }

    internal sealed class AdminBot : IAdminBot
    {
        private const string SubscribeCommand = "/subscribe";
        private const string UnsubscribeCommand = "/unsubscribe";

        private readonly ILogger<AdminBot> _logger;
        private readonly ITelegramBotClient _botClient;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAccidentAlertingService _accidentAlertingService;

        public AdminBot(
            ILogger<AdminBot> logger,
            ITelegramBotClient botClient,
            IAuthorizationService authorizationService,
            IAccidentAlertingService accidentAlertingService)
        {
            _logger = logger;
            _botClient = botClient;
            _authorizationService = authorizationService;
            _accidentAlertingService = accidentAlertingService;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            _logger.LogInformation($"Start handling update {update.Id}");

            if (update is { Type: UpdateType.Message, Message: { Type: MessageType.Text } message })
            {
                var chatId = message.Chat.Id;

                try
                {
                    if (message.TryExtractCommand(out var extractedCommand))
                    {
                        var (command, commandArguments) = extractedCommand.Value;

                        switch (command)
                        {
                            case SubscribeCommand:
                                if (!_authorizationService.VerifySubscriptionSecret(commandArguments))
                                {
                                    return;
                                }

                                var newSubscription = await _accidentAlertingService.SubscribeChatAsync(chatId);

                                var messageFeedback = newSubscription
                                    ? Messages.ChatSubscribedSuccessfully
                                    : Messages.ChatAlreadySubscribed;

                                await messageFeedback.SendAsync(chatId, _botClient);

                                break;

                            case UnsubscribeCommand:
                                if (!_authorizationService.VerifySubscriptionSecret(commandArguments))
                                {
                                    return;
                                }

                                await _accidentAlertingService.UnsubscribeChatAsync(chatId);

                                await Messages.ChatUnsubscribed.SendAsync(chatId, _botClient);

                                break;

                            default:
                                _logger.LogWarning(
                                    $"Skipping message update {update.Id} in chat {chatId}, {command} is not supported"
                                );
                                break;
                        }
                    }
                    else
                    {
                        _logger.LogWarning(
                            $"Skipping message update {update.Id} in chat {chatId}"
                        );
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, $"Error occured while handling update {update.Id}");

                    await Messages.SomethingWentWrong.SendAsync(chatId, _botClient);
                }
                finally
                {
                    _logger.LogInformation($"Finished handling update {update.Id}");
                }
            }
            else
            {
                _logger.LogWarning($"Skipping update {update.Id} as it is not a text message");
            }
        }

        private static class Messages
        {
            public static readonly IMessage ChatSubscribedSuccessfully = MessageFactory.CreateTextMessage()
                .WithPlainText("✅ Этот чат теперь подписан на обновления");

            public static readonly IMessage ChatAlreadySubscribed = MessageFactory.CreateTextMessage()
                .WithMarkdownText("ℹ️ Этот чат *уже* подписан на обновления");

            public static readonly IMessage SomethingWentWrong = MessageFactory
                .CreateTextMessage()
                .WithPlainText("😥 Извините, но что-то пошло не так\n\nПопробуйте ещё раз, если проблема не пройдет, сообщите о ней, пожалуйста, @ndobryanskyy");

            public static readonly IMessage ChatUnsubscribed = MessageFactory.CreateTextMessage()
                .WithPlainText("⛔ Этот чат отписан от обновлений");
        }
    }
}