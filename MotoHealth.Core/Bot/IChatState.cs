﻿namespace MotoHealth.Core.Bot
{
    public interface IChatState
    {
        long AssociatedChatId { get; }

        bool UserSubscribed { get; set; }

        IAccidentReportDialogState? AccidentReportDialog { get; }

        IAccidentReportDialogState StartAccidentReportingDialog(int version);

        void CompleteAccidentReportingDialog();
    }
}