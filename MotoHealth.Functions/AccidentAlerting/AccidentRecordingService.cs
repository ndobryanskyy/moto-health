using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table.Protocol;

namespace MotoHealth.Functions.AccidentAlerting
{
    public interface IAccidentRecordingService
    {
        Task RecordAsync(AccidentTableEntity accident, CancellationToken cancellationToken);
    }

    internal sealed class AccidentRecordingService : IAccidentRecordingService
    {
        private readonly ILogger<AccidentRecordingService> _logger;
        private readonly CloudTable _accidentsTable;

        private bool _isTableInitialized;

        public AccidentRecordingService(ILogger<AccidentRecordingService> logger, ICloudTablesProvider tables)
        {
            _logger = logger;
            _accidentsTable = tables.Accidents;
        }

        public async Task RecordAsync(AccidentTableEntity accident, CancellationToken cancellationToken)
        {
            await EnsureTableExistsAsync(cancellationToken);

            var tableOperation = TableOperation.Insert(accident);

            try
            {
                await _accidentsTable.ExecuteAsync(tableOperation, cancellationToken);

                _logger.LogDebug($"Successfully recorded accident {accident.Id}");
            }
            catch (StorageException exception) when (exception.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.EntityAlreadyExists)
            {
                _logger.LogWarning(exception, $"Accident record for {accident.Id} already exists");
            }
        }

        private async ValueTask EnsureTableExistsAsync(CancellationToken cancellationToken)
        {
            if (_isTableInitialized) return;

            await _accidentsTable.CreateIfNotExistsAsync(cancellationToken);

            _isTableInitialized = true;
        }
    }
}