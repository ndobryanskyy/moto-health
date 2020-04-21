using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MotoHealth.Core.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Core.Bot.Messages
{
    public sealed class TextMessageBuilder : IMessage
    {
        private string? _text;
        private ParseMode _parseMode = ParseMode.Default;
        private IReplyMarkup? _replyMarkup;

        public TextMessageBuilder WithPlainText(string text)
        {
            _parseMode = ParseMode.Default;

            _text = text;

            return this;
        }

        public TextMessageBuilder WithMarkdownText(string text)
        {
            _parseMode = ParseMode.MarkdownV2;

            _text = text;

            return this;
        }

        public TextMessageBuilder WithInterpolatedMarkdownText(FormattableString text, bool escapeInterpolatedValues)
        {
            _parseMode = ParseMode.MarkdownV2;

            if (escapeInterpolatedValues)
            {
                var escapedParameters = text.GetArguments()
                    .Select(x => x?.ToString()?.EscapeForMarkdown())
                    .ToArray();

                // ReSharper disable once CoVariantArrayConversion - read-only usage.
                _text = string.Format(text.Format, escapedParameters);
            }
            else
            {
                _text = text.ToString();
            }

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

        public async Task SendAsync(ChatId chatId, ITelegramBotClient client, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_text))
            {
                // TODO: throw some distinguishable exception
                throw new InvalidOperationException("Cannot send text message with empty text");
            }

            await client.SendTextMessageAsync(chatId, _text, _parseMode, replyMarkup: _replyMarkup, cancellationToken: cancellationToken);
        }
    }
}