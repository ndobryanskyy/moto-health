using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Infrastructure.AzureTables
{
    internal static class TableQueryExtensions
    {
        public static async Task<IReadOnlyList<TResult>> ToListAsync<TResult>(this TableQuery<TResult> query, CancellationToken cancellationToken)
        {
            var result = new List<TResult>();
            TableContinuationToken? continuationToken = null;

            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                var segment = await query.ExecuteSegmentedAsync(continuationToken, cancellationToken);

                result.AddRange(segment);

                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);

            return result;
        }
    }
}