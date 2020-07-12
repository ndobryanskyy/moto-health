using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IChatState
    {
        long AssociatedChatId { get; }

        bool UserSubscribed { get; set; }

        bool UserBanned { get; set; }

        IAccidentReportingDialogState? AccidentReportDialog { get; }

        IAccidentReportingDialogState StartAccidentReportingDialog(int version);

        void CompleteAccidentReportingDialog();
    }
}