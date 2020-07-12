using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.ChatsState.Entities
{
    internal sealed class ChatStateTableEntityAdapter : TableEntityAdapter<ChatState>, IChatState
    {
        /// <summary>
        /// For Serialization
        /// </summary>
        public ChatStateTableEntityAdapter()
        {
        }

        public static ChatStateTableEntityAdapter CreateDefaultForChat(long chatId) 
            => new ChatStateTableEntityAdapter
            {
                PartitionKey = chatId.ToString(),
                RowKey = ChatsTableEntityTypes.State,

                OriginalEntity = new ChatState
                {
                    AssociatedChatId = chatId
                }
            };

        public static TableOperation GetByChatIdRetrieveOperation(long chatId) 
            => TableOperation.Retrieve<ChatStateTableEntityAdapter>(chatId.ToString(), ChatsTableEntityTypes.State);

        #region Explicit Delegating IChatState Implementation

        long IChatState.AssociatedChatId 
            => OriginalEntity.AssociatedChatId;

        bool IChatState.UserSubscribed
        {
            get => OriginalEntity.UserSubscribed;
            set => OriginalEntity.UserSubscribed = value;
        }

        bool IChatState.UserBanned
        {
            get => OriginalEntity.UserBanned;
            set => OriginalEntity.UserBanned = value;
        }

        IAccidentReportingDialogState? IChatState.AccidentReportDialog 
            => OriginalEntity.AccidentReportDialog;

        IAccidentReportingDialogState IChatState.StartAccidentReportingDialog(int version) 
            => OriginalEntity.StartAccidentReportingDialog(version);

        void IChatState.CompleteAccidentReportingDialog() 
            => OriginalEntity.CompleteAccidentReportingDialog();

        #endregion
    }
}