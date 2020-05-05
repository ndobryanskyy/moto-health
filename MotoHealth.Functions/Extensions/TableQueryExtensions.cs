using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace MotoHealth.Functions.Extensions
{
    internal static class TableQueryExtensions
    {
        public static async Task<IReadOnlyList<TResult>> ToListAsync<TResult>(this TableQuery<TResult> query)
        {
            var result = new List<TResult>();
            TableContinuationToken? continuationToken = null;

            do
            {
                var segment = await query.ExecuteSegmentedAsync(continuationToken);

                result.AddRange(segment);

                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);

            return result;
        }
    }
}