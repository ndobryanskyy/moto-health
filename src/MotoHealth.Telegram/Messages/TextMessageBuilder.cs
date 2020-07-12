using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Telegram.Messages
{
    public sealed class TextMessageBuilder : IMessage
    {
        private string? _text;
        private ParseMode _parseMode = ParseMode.Default;
        private IReplyMarkup? _replyMarkup;
        private bool _disableWebPagePreview;

        public TextMessageBuilder WithPlainText(string text)
        {
            _parseMode = ParseMode.Default;

            _text = text;

            return this;
        }

        public TextMessageBuilder WithHtml(string escapedHtml)
        {
            _parseMode = ParseMode.Html;

            _text = escapedHtml;

            return this;
        }

        public TextMessageBuilder WithClearedReplyKeyboard()
        {
            _replyMarkup = new ReplyKeyboardRemove();

            return this;
        }

        public TextMessageBuilder WithReplyKeyboard(
            IEnumerable<IEnumerable<KeyboardButton>> keyboard,
            bool oneTime = true,
            bool fitVertically = true)
        {
            _replyMarkup = new ReplyKeyboardMarkup(keyboard, fitVertically, oneTime);

            return this;
        }

        public TextMessageBuilder WithDisabledWebPagePreview()
        {
            _disableWebPagePreview = true;

            return this;
        }

        async Task IMessage.SendAsync(ChatId chatId, ITelegramClient client, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_text))
            {
                // TODO: throw some distinguishable exception
                throw new InvalidOperationException("Cannot send text message with empty text");
            }

            var request = new SendMessageRequest(chatId, _text)
            {
                DisableNotification = false,
                DisableWebPagePreview = _disableWebPagePreview,
                ParseMode = _parseMode,
                ReplyMarkup = _replyMarkup
            };

            await client.SendTextMessageAsync(request, cancellationToken);
        }
    }
}