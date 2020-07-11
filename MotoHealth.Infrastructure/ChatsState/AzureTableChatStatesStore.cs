using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.AzureTables;
using MotoHealth.Infrastructure.ChatsState.Entities;

namespace MotoHealth.Infrastructure.ChatsState
{
    internal sealed class AzureTableChatStatesStore : IChatStatesStore
    {
        private readonly ICloudTablesProvider _tablesProvider;

        public AzureTableChatStatesStore(ICloudTablesProvider tablesProvider)
        {
            _tablesProvider = tablesProvider;
        }

        public async Task<IChatState> CreateAsync(long chatId, CancellationToken cancellationToken)
        {
            var defaultState = ChatStateTableEntityAdapter.CreateDefaultForChat(chatId);
            var operation = TableOperation.Insert(defaultState);
            var operationResult = await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);

            return (ChatStateTableEntityAdapter) operationResult.Result;
        }

        public async Task<IChatState?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken)
        {
            var operation = ChatStateTableEntityAdapter.GetByChatIdRetrieveOperation(chatId);
            var operationResult = await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);

            return (ChatStateTableEntityAdapter) operationResult.Result;
        }

        public async Task UpdateAsync(IChatState state, CancellationToken cancellationToken)
        {
            var operation = TableOperation.Replace((ChatStateTableEntityAdapter) state);
            await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);
        }
    }
}