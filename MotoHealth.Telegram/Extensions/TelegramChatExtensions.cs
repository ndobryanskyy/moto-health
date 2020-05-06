using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MotoHealth.Telegram.Extensions
{
    public static class TelegramChatExtensions
    {
        public static string GetFriendlyName(this Chat chat)
        {
            var stringBuilder = new StringBuilder();

            if (chat.Type == ChatType.Private)
            {
                if (chat.Username != null)
                {
                    stringBuilder.Append($"@{chat.Username} - ");
                }

                stringBuilder.Append(chat.FirstName);

                if (chat.LastName != null)
                {
                    stringBuilder.Append($" {chat.LastName}");
                }
            }
            else
            {
                stringBuilder.Append($"{chat.Type}: {chat.Title}");

                if (chat.Username != null)
                {
                    stringBuilder.Append($" (@{chat.Username})");
                }
            }

            return stringBuilder.ToString();
        }
    }
}