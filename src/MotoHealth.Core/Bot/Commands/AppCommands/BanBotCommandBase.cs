using System;
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

        private (string BanSecret, int UserId)? _extractedArguments;

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

        protected async Task OnUserIsUnknownAsync(IChatUpdateContext context, int userId, CancellationToken cancellationToken)
        {
            await context.SendMessageAsync(CannotBanOrUnbanUnknownUser, cancellationToken);
            
            Logger.LogWarning($"Tried to ban unknown user: {userId}");
        }

        protected abstract Task ExecuteAsync(IChatUpdateContext context, int userId, CancellationToken cancellationToken);

        protected override bool Matches(ICommandMessageBotUpdate commandMessage)
        {
            var arguments = ExtractArguments(commandMessage);

            return base.Matches(commandMessage) &&
                   arguments.HasValue
                   && _secretsService.VerifyBanSecret(arguments.Value.BanSecret);
        }

        protected sealed override async Task ExecuteAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            var arguments = ExtractArguments((ICommandMessageBotUpdate)context.Update);

            if (context.Update.Sender.Id == arguments!.Value.UserId)
            {
                await context.SendMessageAsync(CannotBanOrUnbanSelf, cancellationToken);
                
                Logger.LogWarning("Tried to ban/unban self");
                return;
            }

            await ExecuteAsync(context, arguments.Value.UserId, cancellationToken);
        }

        private (string BanSecret, int UserId)? ExtractArguments(ICommandMessageBotUpdate commandMessage)
        {
            if (_extractedArguments == null)
            {
                var tokens = commandMessage
                    .Arguments
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 2 &&
                    int.TryParse(tokens[1].Trim(), out var userId))
                {
                    _extractedArguments = (tokens[0].Trim(), userId);
                }
            }

            return _extractedArguments;
        }
    }
}