using System.Collections;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace MotoHealth.Telegram.Messages
{
    public sealed class ReplyKeyboard : IEnumerable<IEnumerable<KeyboardButton>>
    {
        private readonly List<IEnumerable<KeyboardButton>> _keyboardRows = new List<IEnumerable<KeyboardButton>>();

        public void Add(IEnumerable<KeyboardButton> keyboardRow)
        {
            _keyboardRows.Add(keyboardRow);
        }

        public IEnumerator<IEnumerable<KeyboardButton>> GetEnumerator()
        {
            return _keyboardRows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _keyboardRows).GetEnumerator();
        }
    }
}