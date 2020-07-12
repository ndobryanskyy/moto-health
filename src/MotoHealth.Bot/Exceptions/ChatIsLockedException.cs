using System;

namespace MotoHealth.Bot.Exceptions
{
    internal sealed class ChatIsLockedException : Exception
    {
        public ChatIsLockedException(long chatId) : base($"Chat {chatId} is locked")
        {
        }
    }
}