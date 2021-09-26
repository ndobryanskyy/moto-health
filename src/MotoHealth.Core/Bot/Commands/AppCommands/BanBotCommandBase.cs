using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.Updates.Abstractions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    internal abstract class BanBotCommandBase : PrivateChatBotCommandBase
    {
        private static readonly IMessage CannotBanOrUnbanUnknownUser = MessageFactory.CreateTextMessage()
            .WithPlainText("⚠️ Нельзя забанить или разбанить пользователя, которого бот не знает");

        private static readonly IMessage CannotBanOrUnbanSelf = MessageFactory.CreateTextMessage()
            .WithPlainText("⚠️ Нельзя забанить или разбанить себя");

        private readonly IAuthorizationSecretsService _secretsService;

        protected BanBotCommandBase(
            CommandName name, 
            IAuthorizationSecretsService secretsService, 
            IUsersBanService banService,
            IBotTelemetryService telemetryService,
            ILogger logger) 
            : base(name)
        {
            _secretsService = secretsService;

            BanService = banService;
            TelemetryService = telemetryService;
            Logger = logger;
        }

        protected IUsersBanService BanService { get; }

        protected IBotTelemetryService TelemetryService { get; }
        
        protected ILogger Logger { get; }

        protected abstract Task ExecuteAsync(IChatUpdateContext context, long userId, CancellationToken cancellationToken);

        protected sealed override async Task ExecuteAsync(IChatUpdateContext context, ICommandMessageBotUpdate command, CancellationToken cancellationToken)
        {
            if (!TryExtractArguments(command, out var arguments) || !_secretsService.VerifyBanSecret(arguments.Value.Secret))
            {
                await context.SendMessageAsync(CommonMessages.NotQuiteGetIt, cancellationToken);

                Logger.LogWarning("Failed to ban/unban. Invalid arguments");
                return;
            }

            if (command.Sender.Id == arguments.Value.UserId)
            {
                await context.SendMessageAsync(CannotBanOrUnbanSelf, cancellationToken);
                
                Logger.LogWarning("Tried to ban/unban self");
                return;
            }

            await ExecuteAsync(context, arguments.Value.UserId, cancellationToken);
        }

        protected async Task OnUserIsUnknownAsync(IChatUpdateContext context, long userId, CancellationToken cancellationToken)
        {
            await context.SendMessageAsync(CannotBanOrUnbanUnknownUser, cancellationToken);

            Logger.LogWarning($"Tried to ban/unban unknown user: {userId}");
        }

        private bool TryExtractArguments(ICommandMessageBotUpdate commandMessage, [NotNullWhen(true)] out BanCommandArguments? arguments)
        {
            arguments = null;

            var tokens = commandMessage
                .Arguments
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != 2)
            {
                return false;
            }

            var secretToken = tokens[0].Trim();
            var userIdToken = tokens[1].Trim();

            if (!long.TryParse(userIdToken, out var userId))
            {
                return false;
            }

            arguments = new BanCommandArguments(secretToken, userId);
            return true;
        }

        private readonly struct BanCommandArguments
        {
            public BanCommandArguments(string secret, long userId)
            {
                Secret = secret;
                UserId = userId;
            }

            public string Secret { get; }

            public long UserId { get; }
        }
    }
}