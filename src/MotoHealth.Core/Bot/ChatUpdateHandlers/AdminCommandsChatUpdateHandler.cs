using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Common;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.ChatUpdateHandlers
{
    public sealed class AdminCommandsChatUpdateHandler : ChatUpdateHandlerBase
    {
        private const string AccidentsAlertingTopicName = CommonConstants.AccidentReporting.AlertsChatSubscriptionTopicName;

        private readonly IBotTelemetryService _telemetryService;
        private readonly IAdminCommandsChatUpdateHandlerMessages _messages;
        private readonly IBotCommandsRegistry _commandsRegistry;
        private readonly IAuthorizationSecretsService _secretsService;
        private readonly IChatSubscriptionsService _chatSubscriptionsService;
        private readonly IUsersBanService _usersBanService;

        public AdminCommandsChatUpdateHandler(
            IBotTelemetryService telemetryService,
            IAdminCommandsChatUpdateHandlerMessages messages,
            IBotCommandsRegistry commandsRegistry,
            IAuthorizationSecretsService secretsService,
            IChatSubscriptionsService chatSubscriptionsService,
            IUsersBanService usersBanService)
        {
            _telemetryService = telemetryService;
            _messages = messages;
            _commandsRegistry = commandsRegistry;
            _secretsService = secretsService;
            _chatSubscriptionsService = chatSubscriptionsService;
            _usersBanService = usersBanService;
        }

        protected override bool SkipGroupUpdates => false;

        protected override bool SkipHandledUpdates => true;

        protected override async Task OnUpdateAsync(IChatUpdateContext context, ILogger logger, CancellationToken cancellationToken)
        {
            async Task SendMessageAsync(IMessage message)
            {
                await context.SendMessageAsync(message, cancellationToken);
            }

            switch (context.Update)
            {
                case ICommandMessageBotUpdate commandMessage
                     when _commandsRegistry.SubscribeChat.Matches(commandMessage, out var secret) && 
                          _secretsService.VerifySubscriptionSecret(secret):
                    {
                        await _chatSubscriptionsService.SubscribeChatToTopicAsync(context.ChatId, AccidentsAlertingTopicName, cancellationToken);

                        await SendMessageAsync(_messages.ChatWasSubscribed);

                        logger.LogDebug("Chat subscribed to accident alerts");
                        _telemetryService.OnChatSubscribedToAccidentAlerting();

                        context.IsUpdateHandled = true;

                        break;
                    }

                case ICommandMessageBotUpdate commandMessage
                     when _commandsRegistry.UnsubscribeChat.Matches(commandMessage, out var secret) &&
                          _secretsService.VerifySubscriptionSecret(secret):
                    {
                        await _chatSubscriptionsService.UnsubscribeChatFromTopicAsync(context.ChatId, AccidentsAlertingTopicName, cancellationToken);

                        await SendMessageAsync(_messages.ChatWasUnsubscribed);

                        logger.LogDebug("Chat unsubscribed from accident alerts");
                        _telemetryService.OnChatUnsubscribedFromAccidentAlerting();

                        context.IsUpdateHandled = true;

                        break;
                    }

                case ICommandMessageBotUpdate commandMessage
                    when _commandsRegistry.BanUser.Matches(commandMessage, out var arguments) &&
                         arguments.Value is { Secret: var secret, UserId: var userId } &&
                         _secretsService.VerifyBanSecret(secret):
                    {
                        if (userId == context.Update.Sender.Id)
                        {
                            await SendMessageAsync(_messages.CannotBanOrUnbanSelf);
                        }
                        else
                        {
                            var result = await _usersBanService.BanUserAsync(userId, cancellationToken);

                            switch (result)
                            {
                                case BanOperationResult.UserIsUnknown:
                                {
                                    await SendMessageAsync(_messages.CannotBanOrUnbanUnknownUser);
                                    break;
                                }
                                case BanOperationResult.CurrentStateMatchesDesired:
                                {
                                    await SendMessageAsync(_messages.UserIsAlreadyBanned(userId));
                                    break;
                                }
                                case BanOperationResult.Success:
                                {
                                    await SendMessageAsync(_messages.UserWasBanned(userId));
                                    
                                    _telemetryService.OnUserBanned(userId);

                                    break;
                                }
                            }
                        }

                        context.IsUpdateHandled = true;

                        break;
                    }

                case ICommandMessageBotUpdate commandMessage
                    when _commandsRegistry.UnbanUser.Matches(commandMessage, out var arguments) &&
                         arguments.Value is { Secret: var secret, UserId: var userId } &&
                         _secretsService.VerifyBanSecret(secret):
                    {
                        if (userId == context.Update.Sender.Id)
                        {
                            await SendMessageAsync(_messages.CannotBanOrUnbanSelf);
                        }
                        else
                        {
                            var result = await _usersBanService.UnbanUserAsync(userId, cancellationToken);

                            switch (result)
                            {
                                case BanOperationResult.UserIsUnknown:
                                {
                                    await SendMessageAsync(_messages.CannotBanOrUnbanUnknownUser);
                                    break;
                                }
                                case BanOperationResult.CurrentStateMatchesDesired:
                                {
                                    await SendMessageAsync(_messages.UserIsNotBanned(userId));
                                    break;
                                }
                                case BanOperationResult.Success:
                                {
                                    await SendMessageAsync(_messages.UserWasUnbanned(userId));
                                    
                                    _telemetryService.OnUserUnbanned(userId);

                                    break;
                                }
                            }
                        }

                        context.IsUpdateHandled = true;

                        break;
                    }
            }
        }
    }
}