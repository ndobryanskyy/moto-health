using System;
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

        public async Task<IChatState?> GetByChatIdAsync(long chatId, CancellationToken cancellationToken)
        {
            var operation = TableOperation.Retrieve<TableEntityAdapter<ChatState>>(chatId.ToString(), ChatsTableEntityTypes.State);
            var executionResult = await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);

            var result = executionResult.Result;

            if (result is TableEntityAdapter<ChatState> adapter)
            {
                return adapter.OriginalEntity;
            }

            return null;
        }

        public async Task AddAsync(IChatState state, CancellationToken cancellationToken)
        {
            if (state is ChatState entity)
            {
                var operation = TableOperation.InsertOrReplace(entity.ToTableEntity());
                await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"State must be of type {nameof(ChatState)}");
            }
        }

        public async Task UpdateAsync(IChatState state, CancellationToken cancellationToken)
        {
            if (state is ChatState entity)
            {
                var operation = TableOperation.Replace(entity.ToTableEntity());
                await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"State must be of type {nameof(ChatState)}");
            }
        }
    }
}