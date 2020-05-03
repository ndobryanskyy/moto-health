using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class BotUpdateContextTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IBotUpdateAccessor _botUpdateAccessor;

        public BotUpdateContextTelemetryInitializer(IBotUpdateAccessor botUpdateAccessor)
        {
            _botUpdateAccessor = botUpdateAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            var botUpdate = _botUpdateAccessor.Current;

            if (botUpdate != null)
            {
                telemetry.Context.Operation.Id = botUpdate.UpdateId.ToString();
                telemetry.Context.Operation.Name = "Handle Telegram Update";

                if (botUpdate is IHasSender botUpdateWithSender)
                {
                    var sender = botUpdateWithSender.Sender;

                    telemetry.Context.User.Id = sender.Id.ToString();
                    telemetry.Context.User.AuthenticatedUserId = GetAuthenticatedUserId(sender);
                }

                if (telemetry is ISupportProperties telemetryWithProperties)
                {
                    if (botUpdate is IBelongsToChat botUpdateForChat)
                    {
                        var chat = botUpdateForChat.Chat;

                        telemetryWithProperties.Properties["Chat Id"] = chat.Id.ToString();
                        telemetryWithProperties.Properties["Chat Type"] = chat.Type.ToString();
                    }
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
