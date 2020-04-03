using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Infrastructure.ChatStorage.Entities;

namespace MotoHealth.Infrastructure.ChatStorage
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
            var operation = TableOperation.Retrieve<ChatStateTableEntity>(chatId.ToString(), ChatsTableEntityTypes.State);
            var executionResult = await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);

            var result = executionResult.Result;

            return (IChatState?) result;
        }

        public async Task AddAsync(IChatState state, CancellationToken cancellationToken)
        {
            if (state is ChatStateTableEntity tableEntity)
            {
                var operation = TableOperation.InsertOrMerge(tableEntity);
                await _tablesProvider.Chats.ExecuteAsync(operation, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException($"State must be of type {nameof(ChatStateTableEntity)}");
            }
        }
    }
}