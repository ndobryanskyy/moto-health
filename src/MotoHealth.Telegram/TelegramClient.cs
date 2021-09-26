using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoHealth.Telegram.Exceptions;
using Newtonsoft.Json;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace MotoHealth.Telegram
{
    internal sealed class TelegramClient : ITelegramClient
    {
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();

        private readonly ILogger<TelegramClient> _logger;
        private readonly HttpClient _client;

        public TelegramClient(ILogger<TelegramClient> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<User> GetMeAsync(CancellationToken cancellationToken)
        {
            using var response = await SendAsync(new GetMeRequest(), HttpCompletionOption.ResponseContentRead, cancellationToken);

            var telegramResponse = await DeserializeTelegramResponseAsync<User>(response);
            return telegramResponse.Result;
        }

        public async Task SendTextMessageAsync(SendMessageRequest request, CancellationToken cancellationToken)
        {
            using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }

        public async Task SendVenueMessageAsync(SendVenueRequest request, CancellationToken cancellationToken)
        {
            using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }

        public async Task SetBotCommandsAsync(SetMyCommandsRequest request, CancellationToken cancellationToken)
        {
            using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }

        public async Task SetWebhookAsync(SetWebhookRequest request, CancellationToken cancellationToken)
        {
            using var response = await SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }

        private async Task<HttpResponseMessage> SendAsync<TResponse>(
            IRequest<TResponse> telegramRequest, 
            HttpCompletionOption completion, 
            CancellationToken cancellationToken)
        {
            using var httpRequest = new HttpRequestMessage(telegramRequest.Method, telegramRequest.MethodName)
            {
                Content = telegramRequest.ToHttpContent()
            };

            var response = await _client.SendAsync(httpRequest, completion, cancellationToken);
            await EnsureSuccessResponseAsync(response);

            return response;
        }

        private async ValueTask EnsureSuccessResponseAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return;

            var telegramResponse = await DeserializeTelegramResponseAsync<object>(response);

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

        private async Task<ApiResponse<TResult>> DeserializeTelegramResponseAsync<TResult>(HttpResponseMessage response)
        {
            try
            {
                var responseString = await response.Content.ReadAsStringAsync();

                using var streamReader = new StringReader(responseString);
                using var jsonReader = new JsonTextReader(streamReader);

                return JsonSerializer.Deserialize<ApiResponse<TResult>>(jsonReader);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to deserialize telegram response");
                throw;
            }
        }
    }
}