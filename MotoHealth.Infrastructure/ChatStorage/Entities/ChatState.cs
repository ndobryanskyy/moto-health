using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot;

namespace MotoHealth.Infrastructure.ChatStorage.Entities
{
    internal sealed class ChatState : IChatState
    {
        public long AssociatedChatId { get; set; }

        public int AssociatedUserId { get; set; }

        public bool UserSubscribed { get; set; }

        [IgnoreProperty]
        IAccidentReportDialogState? IChatState.AccidentReportDialog => AccidentReportDialog;

        public AccidentReportDialogState? AccidentReportDialog { get; set; }

        public IAccidentReportDialogState StartAccidentReportingDialog(int version)
        {
            AccidentReportDialog = new AccidentReportDialogState
            {
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

            public int CurrentStep { get; set; }

            public string Address { get; set; } = string.Empty;

            public string Participants { get; set; } = string.Empty;

            public string Victims { get; set; } = string.Empty;
            
            public string? ReporterPhoneNumber { get; set; }
        }
    }
}