using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    internal sealed class UnbanUserBotCommand : BanBotCommandBase
    {
        private static IMessage UserIsNotBanned(long userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"⚠️ Пользователь {TelegramHtml.UserLink(userId, userId.ToString(CultureInfo.InvariantCulture))} уже разбанен");

        private static IMessage UserWasUnbanned(long userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"✅ Пользователь {TelegramHtml.UserLink(userId, userId.ToString(CultureInfo.InvariantCulture))} разбанен");

        public UnbanUserBotCommand(
            IAuthorizationSecretsService secretsService, 
            IUsersBanService banService,
            IBotTelemetryService telemetryService,
            ILogger<UnbanUserBotCommand> logger) 
            : base("/unban", secretsService, banService, telemetryService, logger)
        {
        }

        protected override async Task ExecuteAsync(IChatUpdateContext context, long userId, CancellationToken cancellationToken)
        {
            var result = await BanService.UnbanUserAsync(userId, cancellationToken);

            switch (result)
            {
                case BanOperationResult.UserIsUnknown:
                    await OnUserIsUnknownAsync(context, userId, cancellationToken);
                    break;

                case BanOperationResult.CurrentStateMatchesDesired:
                    await context.SendMessageAsync(UserIsNotBanned(userId), cancellationToken);
                    break;

                case BanOperationResult.Success:
                    await context.SendMessageAsync(UserWasUnbanned(userId), cancellationToken);

                    TelemetryService.OnUserUnbanned(userId);
                    break;
            }
        }
    }
}