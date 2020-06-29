using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Telegram;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace MotoHealth.Bot.Middleware
{
    public sealed class BotUpdateInitializerMiddleware : IMiddleware
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        private readonly ILogger<BotUpdateInitializerMiddleware> _logger;
        private readonly IBotUpdatesMapper _updatesMapper;

        public BotUpdateInitializerMiddleware(
            ILogger<BotUpdateInitializerMiddleware> logger,
            IBotUpdatesMapper updatesMapper)
        {
            _logger = logger;
            _updatesMapper = updatesMapper;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var memoryStream = await ReadRequestBodyAsync(context);
            var update = DeserializeUpdate(memoryStream);
            
            _logger.LogDebug($"Successfully deserialized update {update.Id}");

            var botUpdate = _updatesMapper.MapTelegramUpdate(update);

            context.SetBotUpdate(botUpdate);

            _logger.LogInformation($"Received update {botUpdate.UpdateId} {botUpdate.GetType().Name}");

            await next(context);
        }

        private static async Task<MemoryStream> ReadRequestBodyAsync(HttpContext context)
        {
            var memoryStream = new MemoryStream();
            await context.Request.Body.CopyToAsync(memoryStream, context.RequestAborted);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }

        private static Update DeserializeUpdate(Stream stream)
        {
            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);

            var update = JsonSerializer.Deserialize<Update>(jsonReader);

            if (update == null)
            {
                throw new ArgumentException("Request body contains nothing");
            }

            return update;
        }
    }
}