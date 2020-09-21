using System;
using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.AccidentReporting.Exceptions
{
    public class AccidentReportingDialogReplyException : Exception
    {
        public AccidentReportingDialogReplyException(IMessage userFriendlyErrorMessage)
        {
            UserFriendlyErrorMessage = userFriendlyErrorMessage;
        }

        public IMessage UserFriendlyErrorMessage { get; }
    }
}