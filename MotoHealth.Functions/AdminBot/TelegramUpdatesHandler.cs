using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Telegram.Messages;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Functions.AdminBot
{
    public sealed class TelegramUpdatesHandler
    {
        private readonly IBotTokenValidator _botTokenValidator;
        private readonly ITelegramBotClient _botClient;

        private readonly JsonSerializer _newtonsoftSerializer = new JsonSerializer();

        public TelegramUpdatesHandler(
            IBotTokenValidator botTokenValidator,
            ITelegramBotClient botClient)
        {
            _botTokenValidator = botTokenValidator;
            _botClient = botClient;
        }

        [FunctionName("AdminBotUpdatesHandler")]
        public async Task<IActionResult> HandleUpdateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "updates")] HttpRequest request,
            ILogger logger)
        {
            if (!_botTokenValidator.ValidateToken(request))
            {
                return new NotFoundResult();
            }

            try
            {
                var update = DeserializeUpdate(request.Body);

                if (update is { Type: UpdateType.Message, Message: { Type: MessageType.Text } message })
                {
                    await MessageFactory.CreateTextMessage()
                        .WithPlainText($"Echo: {message.Text}")
                        .SendAsync(message.Chat.Id, _botClient, request.HttpContext.RequestAborted);
                }
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Error, while handling update");
            }

            return new OkResult();
        }

        private Update DeserializeUpdate(Stream body)
        {
            using var streamReader = new StreamReader(body);
            using var jsonReader = new JsonTextReader(streamReader);

            return _newtonsoftSerializer.Deserialize<Update>(jsonReader);
        }
    }
}