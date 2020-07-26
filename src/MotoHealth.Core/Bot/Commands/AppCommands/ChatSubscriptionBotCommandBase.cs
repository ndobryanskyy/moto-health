using System;
using System.Linq;
using MotoHealth.Common;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    internal abstract class ChatSubscriptionBotCommandBase : BotCommandBase
    {
        protected const string AccidentAlertingTopicName = CommonConstants.AccidentReporting.AlertsChatSubscriptionTopicName;

        private readonly IAuthorizationSecretsService _secretsService;

        protected ChatSubscriptionBotCommandBase(
            CommandName name, 
            IAuthorizationSecretsService secretsService,
            IChatSubscriptionsService subscriptionsService,
            IBotTelemetryService telemetryService) 
            : base(name)
        {
            _secretsService = secretsService;

            SubscriptionsService = subscriptionsService;
            TelemetryService = telemetryService;
        }

        protected IChatSubscriptionsService SubscriptionsService { get; }
        
        protected IBotTelemetryService TelemetryService { get; }

        protected override bool Matches(ICommandMessageBotUpdate commandMessage)
        {
            var tokens = commandMessage.Arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return base.Matches(commandMessage) &&
                   tokens.Length == 1 &&
                   _secretsService.VerifySubscriptionSecret(tokens.First().Trim());
        }
    }
}