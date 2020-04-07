using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Core.Bot.Messages
{
    internal sealed class TextMessageBuilder : IMessage
    {
        private readonly string _text;
        private ParseMode _parseMode = ParseMode.Default;
        private IReplyMarkup? _replyMarkup;

        public TextMessageBuilder(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                // TODO: throw some distinguishable exception
                throw new InvalidOperationException("Message text must not be empty");
            }

            _text = text;
        }

        public TextMessageBuilder ParseAsMarkdown()
        {
            _parseMode = ParseMode.MarkdownV2;

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
            => await client.SendTextMessageAsync(chatId, _text, _parseMode, replyMarkup: _replyMarkup, cancellationToken: cancellationToken);
    }
}