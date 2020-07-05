using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

namespace MotoHealth.Bot.AppInsights
{
    public interface ITelegramTelemetrySanitizer
    {
        string SanitizeBotApiRequestUrl(string url);

        Uri SanitizeWebhookUri(Uri uri);
    }

    internal sealed class TelegramTelemetrySanitizer : ITelegramTelemetrySanitizer
    {
        private static readonly Regex TelegramRequestUrlBotTokenReplaceRegex =
            new Regex(@"bot(.+):(.+)/", RegexOptions.Compiled);

        public string SanitizeBotApiRequestUrl(string url) 
            => TelegramRequestUrlBotTokenReplaceRegex.Replace(url, "bot***/");

        public Uri SanitizeWebhookUri(Uri uri)
        {
            NameValueCollection? query;

            try
            {
                query = HttpUtility.ParseQueryString(uri.Query);
            }
            catch (Exception)
            {
                return uri;
            }

            if (query == null)
            {
                return uri;
            }

            if (query.Get(Constants.Telegram.BotIdQueryParamName) != null)
            {
                query.Set(Constants.Telegram.BotIdQueryParamName, "***");
            }

            if (query.Get(Constants.Telegram.BotSecretQueryParamName) != null)
            {
                query.Set(Constants.Telegram.BotSecretQueryParamName, "***");
            }

            var builder = new UriBuilder(uri)
            {
                Query = query.ToString()
            };

            return builder.Uri;
        }
    }
}