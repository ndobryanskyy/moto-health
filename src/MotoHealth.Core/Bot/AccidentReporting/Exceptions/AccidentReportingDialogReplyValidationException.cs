using MotoHealth.Telegram.Messages;

namespace MotoHealth.Core.Bot.AccidentReporting.Exceptions
{
    public sealed class AccidentReportingDialogReplyValidationException : AccidentReportingDialogReplyException
    {
        public AccidentReportingDialogReplyValidationException(IMessage userFriendlyErrorMessage)
            : base(userFriendlyErrorMessage)
        {
        }
    }
}