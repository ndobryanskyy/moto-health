using System;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.AccidentReporting.Exceptions
{
    public sealed class ReplyValidationException : Exception
    {
        public IMessage UserFriendlyErrorMessage { get; }

        public ReplyValidationException(IMessage userFriendlyErrorMessage)
        {
            UserFriendlyErrorMessage = userFriendlyErrorMessage;
        }
    }
}