using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace MotoHealth.Functions.AdminBot
{
    public sealed class TelegramUpdatesHandler
    {
        private readonly IBotTokenValidator _botTokenValidator;
        private readonly IAdminBot _adminBot;
        private readonly ILogger<TelegramUpdatesHandler> _logger;

        private readonly JsonSerializer _newtonsoftSerializer = new JsonSerializer();

        public TelegramUpdatesHandler(
            IBotTokenValidator botTokenValidator,
            IAdminBot adminBot,
            ILogger<TelegramUpdatesHandler> logger)
        {
            _botTokenValidator = botTokenValidator;
            _adminBot = adminBot;
            _logger = logger;
        }

        [FunctionName(FunctionNames.AdminBotUpdatesHandler)]
        public async Task<IActionResult> HandleUpdateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "updates")] HttpRequest request)
        {
            if (!_botTokenValidator.ValidateToken(request))
            {
                return new NotFoundResult();
            }

            if (TryDeserializeUpdate(request.Body, out var update))
            {
                try
                {
                    await _adminBot.HandleUpdateAsync(update);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Unhandled error, while handling update {update.Id}");
                }
            }

            return new OkResult();
        }

        private bool TryDeserializeUpdate(Stream body, [NotNullWhen(true)] out Update? update)
        {
            update = null;

            try
            {
                using var streamReader = new StreamReader(body);
                using var jsonReader = new JsonTextReader(streamReader);

                update = _newtonsoftSerializer.Deserialize<Update>(jsonReader);
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error, while deserializing update");
                return false;
            }
        }
    }
}