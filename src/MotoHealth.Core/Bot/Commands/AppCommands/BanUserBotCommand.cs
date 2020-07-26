using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    internal sealed class BanUserBotCommand : BanBotCommandBase
    {
        private static IMessage UserIsAlreadyBanned(int userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"⚠️ Пользователь {TelegramHtml.UserLink(userId, userId.ToString())} уже забанен");

        private static IMessage UserWasBanned(int userId) => MessageFactory.CreateTextMessage()
            .WithHtml($"⛔ Пользователь {TelegramHtml.UserLink(userId, userId.ToString())} забанен");

        public BanUserBotCommand(
            IAuthorizationSecretsService secretsService,
            IUsersBanService banService,
            IBotTelemetryService telemetryService,
            ILogger<BanUserBotCommand> logger) 
            : base("/ban", secretsService, banService, telemetryService, logger)
        {
        }

        protected override async Task ExecuteAsync(IChatUpdateContext context, int userId, CancellationToken cancellationToken)
        {
            var result = await BanService.BanUserAsync(userId, cancellationToken);

            switch (result)
            {
                case BanOperationResult.UserIsUnknown:
                    await OnUserIsUnknownAsync(context, userId, cancellationToken);
                    break;

                case BanOperationResult.CurrentStateMatchesDesired:
                    await context.SendMessageAsync(UserIsAlreadyBanned(userId), cancellationToken);
                    break;

                case BanOperationResult.Success:
                    await context.SendMessageAsync(UserWasBanned(userId), cancellationToken);

                    TelemetryService.OnUserBanned(userId);
                    break;
            }
        }
    }
}