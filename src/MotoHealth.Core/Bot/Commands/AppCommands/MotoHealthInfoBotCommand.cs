using System;
using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.Commands.AppCommands
{
    [PublicBotCommand(Name, "О нас")]
    internal sealed class MotoHealthInfoBotCommand : PrivateChatBotCommandBase
    {
        private const string Name = "/info";

        private static readonly IMessage MotoHealthInfo = MessageFactory.CreateTextMessage()
            .WithHtml(
                "🚑 <b>MOTO HEALTH</b>\n\n" +
                "<b>Телефон:</b> +380960543434\n" +
                $"<b>Сайт:</b> {TelegramHtml.Link(new Uri("http://www.mh.od.ua"), "mh.od.ua")}\n" +
                $"<b>Instagram:</b> {TelegramHtml.Link(new Uri("https://www.instagram.com/moto_health_odessa"), "@moto_health_odessa")}")
            .WithDisabledWebPagePreview();

        private readonly IBotTelemetryService _telemetryService;

        public MotoHealthInfoBotCommand(IBotTelemetryService telemetryService)
            : base(Name)
        {
            _telemetryService = telemetryService;
        }

        protected override async Task ExecuteAsync(IChatUpdateContext context, CancellationToken cancellationToken)
        {
            await context.SendMessageAsync(MotoHealthInfo, cancellationToken);

            _telemetryService.OnMotoHealthInfoProvided();
        }
    }
}