using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MotoHealth.PubSub.EventData;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Exceptions;
using MotoHealth.Telegram.Extensions;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class AlertChatActivity
    {
        private static readonly TimeZoneInfo UkraineTimezone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

        private readonly ILogger<AlertChatActivity> _logger;
        private readonly ITelegramClient _telegramClient;

        public AlertChatActivity(ILogger<AlertChatActivity> logger, ITelegramClient telegramClient)
        {
            _logger = logger;
            _telegramClient = telegramClient;
        }

        [FunctionName(FunctionNames.AccidentAlerting.AlertChatActivity)]
        public async Task<AlertChatActivityOutput> RunAsync([ActivityTrigger] AlertChatActivityInput input)
        {
            var chatId = input.ChatId;
            var reportId = input.AccidentReport.ReportId;

            var alert = CreateAccidentAlert(input.AccidentReport);

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

        private static IMessage CreateAccidentAlert(AccidentReportedEventData accidentReport)
        {
            var reportedAtLocalTime = TimeZoneInfo.ConvertTimeFromUtc(accidentReport.ReportedAtUtc, UkraineTimezone);

            var accidentLocation = accidentReport.AccidentLocation;
            var address = accidentLocation != null
                ? @$"<a href=""{BuildGoogleMapsLink(accidentLocation)}"">Геопозиция</a>"
                : accidentReport.AccidentAddress?.HtmlEscaped() ?? "Не указан";

            const string alertBorder = "🚨🚨🚨🚨🚨🚨🚨🚨🚨";

            return MessageFactory.CreateTextMessage().WithHtml(
                $"{alertBorder}\n\n" +
                "<b>СООБЩЕНИЕ О ДТП</b>\n\n" +

                $"<b>Адрес:</b> {address}\n" +
                $"<b>Участник:</b> {accidentReport.AccidentParticipant.HtmlEscaped()}\n" +
                $"<b>Пострадавшие:</b> {accidentReport.AccidentVictims.HtmlEscaped()}\n" +
                $"<b>Телефон:</b> {accidentReport.ReporterPhoneNumber.HtmlEscaped()}\n\n" +

                $"<b>Получено:</b> <i>{reportedAtLocalTime:dd/MM - HH:mm:ss}</i>\n" +
                @$"<a href=""{BuildUserMentionLink(accidentReport.ReporterTelegramUserId)}"">Отправитель | {accidentReport.ReporterTelegramUserId}</a>" +
                $"\n\n{alertBorder}"
            ).WithDisabledWebPagePreview();
        }

        private static string BuildGoogleMapsLink(MapLocation location) 
            => $"https://www.google.com/maps/search/?api=1&query={location.Latitude},{location.Longitude}";

        private static string BuildUserMentionLink(int userId)
            => $"tg://user?id={userId}";
    }
}