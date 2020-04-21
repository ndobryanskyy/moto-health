using System;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.ChatStorage.Entities
{
    internal sealed class ChatState : IChatState
    {
        public long AssociatedChatId { get; set; }

        public bool UserSubscribed { get; set; }

        [IgnoreProperty]
        IAccidentReportDialogState? IChatState.AccidentReportDialog => AccidentReportDialog;

        public AccidentReportDialogState? AccidentReportDialog { get; set; }

        public IAccidentReportDialogState StartAccidentReportingDialog(int version)
        {
            AccidentReportDialog = new AccidentReportDialogState
            {
                InstanceId = Guid.NewGuid().ToString("D"),
                Version = version,
                CurrentStep = 1
            };

            return AccidentReportDialog;
        }

        public void CompleteAccidentReportingDialog()
        {
            AccidentReportDialog = null;
        }

        public TableEntity ToTableEntity() 
            => new TableEntityAdapter<ChatState>(this, AssociatedChatId.ToString(), ChatsTableEntityTypes.State) { ETag = "*" };

        public class AccidentReportDialogState : IAccidentReportDialogState
        {
            public int Version { get; set; }

            public string InstanceId { get; set; } = string.Empty;

            public int CurrentStep { get; set; }

            public string Address { get; set; } = string.Empty;

            public string Participants { get; set; } = string.Empty;

            public string Victims { get; set; } = string.Empty;
            
            public string? ReporterPhoneNumber { get; set; }
        }
    }
}