using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Exceptions;
using MotoHealth.Telegram.Extensions;
using MotoHealth.Telegram.Messages;
using TimeZoneConverter;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class AlertChatActivity
    {
        private static readonly TimeZoneInfo UkraineTimezone = TZConvert.GetTimeZoneInfo("FLE Standard Time");

        private readonly ILogger<AlertChatActivity> _logger;
        private readonly ITelegramClient _telegramClient;
        private readonly IGoogleMapsService _googleMapsService;

        public AlertChatActivity(
            ILogger<AlertChatActivity> logger,
            ITelegramClient telegramClient,
            IGoogleMapsService googleMapsService)
        {
            _logger = logger;
            _telegramClient = telegramClient;
            _googleMapsService = googleMapsService;
        }

        [FunctionName(Constants.FunctionNames.AccidentAlerting.AlertChatActivity)]
        public async Task<AlertChatActivityOutput> RunAsync([ActivityTrigger] AlertChatActivityInput input)
        {
            var chatId = input.ChatId;
            var reportId = input.AccidentReport.Id;

            var alert = CreateAccidentAlertMessage(input.AccidentReport);

            try
            {
                await alert.SendAsync(chatId, _telegramClient);

                _logger.LogInformation($"Successfully alerted chat {chatId} about report {reportId}");

                return new AlertChatActivityOutput
                {
                    AlertSent = true
                };
            }
            catch (TelegramApiException exception) when (exception.Type == TelegramApiError.Forbidden)
            {
                _logger.LogWarning(exception, $"Skipped alerting chat {chatId} about report {reportId}");
            }
            catch (TelegramApiException exception) when (exception.Type == TelegramApiError.BadRequest)
            {
                _logger.LogError(exception, $"Unexpected client error, while alerting {chatId} about report {reportId}");
            }

            return new AlertChatActivityOutput
            {
                AlertSent = false
            };
        }

        private IMessage CreateAccidentAlertMessage(AccidentAlertingWorkflowInput.AccidentReportSummary accidentReport)
        {
            var reportedAtLocalTime = TimeZoneInfo.ConvertTimeFromUtc(accidentReport.ReportedAtUtc, UkraineTimezone);

            var addressText = accidentReport switch
            {
                { AccidentLocation: var location } when location != null 
                    => TelegramHtml.Link(_googleMapsService.GetLocationPinUri(location.Latitude, location.Longitude), "Геопозиция"),
                { AccidentAddress: var address } when !string.IsNullOrEmpty(address)
                    => address.HtmlEscaped(),
                _ 
                    => "Не указан"
            };

            const string alertBorder = "🚨🚨🚨🚨🚨🚨🚨🚨🚨";

            return MessageFactory.CreateTextMessage().WithHtml(
                $"{alertBorder}\n\n" +
                "<b>СООБЩЕНИЕ О ДТП</b>\n\n" +

                $"<b>Адрес:</b> {addressText}\n" +
                $"<b>Участник:</b> {accidentReport.AccidentParticipant.HtmlEscaped()}\n" +
                $"<b>Пострадавшие:</b> {accidentReport.AccidentVictims.HtmlEscaped()}\n" +
                $"<b>Телефон:</b> {accidentReport.ReporterPhoneNumber.HtmlEscaped()}\n\n" +

                $"<b>Получено:</b> <i>{reportedAtLocalTime:dd/MM - HH:mm:ss}</i>\n" +
                @$"{TelegramHtml.UserLink(accidentReport.ReporterTelegramUserId, $"Отправитель | {accidentReport.ReporterTelegramUserId:D}")}" +
                $"\n\n{alertBorder}"
            ).WithDisabledWebPagePreview();
        }
    }
}