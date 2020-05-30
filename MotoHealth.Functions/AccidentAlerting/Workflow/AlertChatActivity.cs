using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MotoHealth.PubSub.EventData;
using MotoHealth.Telegram;
using MotoHealth.Telegram.Exceptions;
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
            var compositeMessage = MessageFactory.CreateCompositeMessage();

            var reportDateTimeFormatted = TimeZoneInfo.ConvertTimeFromUtc(accidentReport.ReportedAtUtc, UkraineTimezone)
                .ToString("dd/MM - HH:mm:ss");

            var locationSpecified = accidentReport.AccidentLocation != null;

            var address = accidentReport.AccidentAddress ?? (locationSpecified ? "Геопозиция" : "Не указан");

            var alert = MessageFactory.CreateTextMessage()
                .WithInterpolatedMarkdownText(
@$"🚨🚨🚨🚨🚨🚨🚨

 *СООБЩЕНИЕ О ДТП*
 _Номер:_ *{accidentReport.ReportId}*
 _Получено: {reportDateTimeFormatted}_

 *Адрес:* {address}
 *Участник:* {accidentReport.AccidentParticipant}
 *Пострадавшие:* {accidentReport.AccidentVictims}
 *Телефон сообщившего:* {accidentReport.ReporterPhoneNumber}

🚨🚨🚨🚨🚨🚨🚨");

            compositeMessage.AddMessage(alert);

            if (locationSpecified)
            {
                var location = MessageFactory.CreateVenueMessage()
                    .WithLocation(accidentReport.AccidentLocation!.Latitude, accidentReport.AccidentLocation.Longitude)
                    .WithTitle($"ДТП {accidentReport.ReportId}");

                compositeMessage.AddMessage(location);
            }

            return compositeMessage;
        }
    }
}