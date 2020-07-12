using System;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.ChatsState.Entities
{
    internal sealed class ChatState : IChatState
    {
        public long AssociatedChatId { get; set; }

        public bool UserSubscribed { get; set; }

        public bool UserBanned { get; set; }

        [IgnoreProperty]
        IAccidentReportingDialogState? IChatState.AccidentReportDialog => AccidentReportDialog;

        public AccidentReportingDialogState? AccidentReportDialog { get; set; }

        public IAccidentReportingDialogState StartAccidentReportingDialog(int version)
        {
            if (AccidentReportDialog != null)
            {
                throw new InvalidOperationException();
            }

            AccidentReportDialog = new AccidentReportingDialogState
            {
                InstanceId = Guid.NewGuid().ToString(),
                ReportId = Guid.NewGuid().ToString(),
                StartedAt = DateTimeOffset.UtcNow,
                Version = version,
                CurrentStep = 1
            };

            return AccidentReportDialog;
        }

        public void CompleteAccidentReportingDialog()
        {
            AccidentReportDialog = null;
        }
    }
}