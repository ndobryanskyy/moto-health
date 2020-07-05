using System.Globalization;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class BotUpdateContextTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BotUpdateContextTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (_httpContextAccessor.HttpContext.TryGetBotUpdate(out var botUpdate))
            {
                telemetry.Context.Operation.Id = botUpdate.UpdateId.ToString();
                telemetry.Context.Operation.Name = "Handle Telegram Update";

                if (botUpdate is IHasSender botUpdateWithSender)
                {
                    var sender = botUpdateWithSender.Sender;
                    var userId = sender.Id.ToString(CultureInfo.InvariantCulture);

                    telemetry.Context.User.Id = userId;
                    telemetry.Context.User.AuthenticatedUserId = GetAuthenticatedUserId(sender);
                    telemetry.Context.Session.Id = userId;
                }

                if (telemetry is ISupportProperties telemetryWithProperties)
                {
                    if (botUpdate is IBelongsToChat botUpdateForChat)
                    {
                        var chat = botUpdateForChat.Chat;

                        telemetryWithProperties.Properties["Chat Id"] = chat.Id.ToString();
                        telemetryWithProperties.Properties["Chat Type"] = chat.Type.ToString();

                        if (chat.Group != null)
                        {
                            telemetryWithProperties.Properties["Group Title"] = chat.Group.Title;
                        }
                    }
                }

                if (telemetry is RequestTelemetry requestTelemetry)
                {
                    requestTelemetry.Name = "Telegram Webhook";
                    requestTelemetry.Source = "Telegram";
                    requestTelemetry.Properties[TelemetryProperties.WellKnown.UpdateType] = botUpdate.GetUpdateTypeNameForTelemetry();
                }

                if (telemetry is ExceptionTelemetry exceptionTelemetry)
                {
                    botUpdate.ExtractDiagnosticProperties().MergeTo(exceptionTelemetry.Properties);
                }
            }
        }

        private static string GetAuthenticatedUserId(ITelegramUser user)
        {
            var stringBuilder = new StringBuilder();

            if (user.Username != null)
            {
                stringBuilder.Append($"@{user.Username} - ");
            }

            stringBuilder.Append(user.FirstName);

            if (user.LastName != null)
            {
                stringBuilder.Append($" {user.LastName}");
            }

            return stringBuilder.ToString();
        }
    }
}
