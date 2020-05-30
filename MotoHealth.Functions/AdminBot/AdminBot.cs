using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Functions.AccidentAlerting;
using MotoHealth.Functions.Authorization;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Extensions;
using MotoHealth.Telegram.Messages;
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
        private readonly ITelegramClient _telegramClient;
        private readonly IAuthorizationService _authorizationService;
        private readonly IAccidentAlertingSubscriptionsManager _accidentAlertingSubscriptionsManager;

        public AdminBot(
            ILogger<AdminBot> logger,
            ITelegramClient telegramClient,
            IAuthorizationService authorizationService,
            IAccidentAlertingSubscriptionsManager accidentAlertingSubscriptionsManager)
        {
            _logger = logger;
            _telegramClient = telegramClient;
            _authorizationService = authorizationService;
            _accidentAlertingSubscriptionsManager = accidentAlertingSubscriptionsManager;
        }

        public async Task HandleUpdateAsync(Update update)
        {
            _logger.LogInformation($"Start handling update {update.Id}");

            if (update is { Type: UpdateType.Message, Message: { Type: MessageType.Text, Chat: var chat } message })
            {
                var chatId = chat.Id;

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

                                var newSubscription = await _accidentAlertingSubscriptionsManager.SubscribeChatAsync(chat);

                                var messageFeedback = newSubscription
                                    ? Messages.ChatSubscribedSuccessfully
                                    : Messages.ChatAlreadySubscribed;

                                await messageFeedback.SendAsync(chatId, _telegramClient);

                                break;

                            case UnsubscribeCommand:
                                if (!_authorizationService.VerifySubscriptionSecret(commandArguments))
                                {
                                    return;
                                }

                                await _accidentAlertingSubscriptionsManager.UnsubscribeChatAsync(chat);

                                await Messages.ChatUnsubscribed.SendAsync(chatId, _telegramClient);

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

                    await Messages.SomethingWentWrong.SendAsync(chatId, _telegramClient);
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
                .WithPlainText("😥 Извините, что-то пошло не так\n\nПопробуйте ещё раз, если проблема не пройдет, сообщите о ней, пожалуйста, @ndobryanskyy");

            public static readonly IMessage ChatUnsubscribed = MessageFactory.CreateTextMessage()
                .WithPlainText("⛔ Этот чат отписан от обновлений");
        }
    }
}