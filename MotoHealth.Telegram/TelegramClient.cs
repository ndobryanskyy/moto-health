using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.Telegram.Exceptions;
using Newtonsoft.Json;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace MotoHealth.Telegram
{
    internal sealed class TelegramClient : ITelegramClient
    {
        private const string TelegramBaseUrl = "https://api.telegram.org/bot";

        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        private readonly ILogger<TelegramClient> _logger;
        private readonly HttpClient _client;

        public TelegramClient(
            ILogger<TelegramClient> logger,
            IOptions<TelegramOptions> telegramOptions,
            HttpClient client)
        {
            var options = telegramOptions.Value;

            client.Timeout = options.RequestTimeout;
            client.BaseAddress = new Uri($"{TelegramBaseUrl}{options.BotToken}/");

            _logger = logger;
            _client = client;
        }

        public async Task SendTextMessageAsync(SendMessageRequest request, CancellationToken cancellationToken)
        {
            using var httpRequest = ConvertToHttpRequest(request); 
            using var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            
            await EnsureRequestSucceededAsync(response);
        }

        public async Task SendVenueMessageAsync(SendVenueRequest request, CancellationToken cancellationToken)
        {
            using var httpRequest = ConvertToHttpRequest(request);
            using var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            await EnsureRequestSucceededAsync(response);
        }

        public async Task SetBotCommandsAsync(SetMyCommandsRequest request, CancellationToken cancellationToken)
        {
            using var httpRequest = ConvertToHttpRequest(request);
            using var response = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            await EnsureRequestSucceededAsync(response);
        }

        private static HttpRequestMessage ConvertToHttpRequest<TResponse>(IRequest<TResponse> telegramRequest)
        {
            var httpRequest = new HttpRequestMessage(telegramRequest.Method, telegramRequest.MethodName)
            {
                Content = telegramRequest.ToHttpContent()
            };

            return httpRequest;
        }

        private async ValueTask EnsureRequestSucceededAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            var responseContentStream = await response.Content.ReadAsStreamAsync();
            var telegramResponse = DeserializeTelegramResponse(responseContentStream);

            _logger.LogWarning($"Unsuccessful telegram request\nError: {telegramResponse.Description}");

            var error = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => TelegramApiError.BadRequest,
                HttpStatusCode.Forbidden => TelegramApiError.Forbidden,
                HttpStatusCode.InternalServerError => TelegramApiError.InternalServerError,
                _ => TelegramApiError.Unexpected
            };

            throw new TelegramApiException(error, telegramResponse.Description);
        }

        private ApiResponse<object> DeserializeTelegramResponse(Stream stream)
        {
            try
            {
                using var streamReader = new StreamReader(stream);
                using var jsonReader = new JsonTextReader(streamReader);

                return JsonSerializer.Deserialize<ApiResponse<object>>(jsonReader);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to deserialize telegram response");
                throw;
            }
        }
    }
}