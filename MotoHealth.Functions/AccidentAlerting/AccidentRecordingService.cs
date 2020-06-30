using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using MotoHealth.Functions.AccidentAlerting.Workflow;

namespace MotoHealth.Functions.AccidentAlerting
{
    public interface IAccidentRecordingService
    {
        Task RecordAsync(RecordAccidentActivityInput activityInput);
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

        public async Task RecordAsync(RecordAccidentActivityInput activityInput)
        {
            await EnsureTableExistsAsync();

            var entity = AccidentTableEntity.CreateFromReportDto(activityInput.AccidentReport);

            entity.HandledAtUtc = activityInput.ReportHandledAtUtc;
            entity.AnyChatAlerted = activityInput.AnyChatAlerted;

            var tableOperation = TableOperation.Insert(entity);

            try
            {
                await _accidentsTable.ExecuteAsync(tableOperation);

                _logger.LogInformation($"Successfully recorded accident {activityInput.AccidentReport.Id} from {activityInput.AccidentReport.ReporterTelegramUserId}");
            }
            catch (StorageException exception) when (exception.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.EntityAlreadyExists)
            {
                _logger.LogWarning(exception, $"Accident record for {activityInput.AccidentReport.Id} already exists");
            }
        }

        private async ValueTask EnsureTableExistsAsync()
        {
            if (_isTableInitialized) return;

            await _accidentsTable.CreateIfNotExistsAsync();

            _isTableInitialized = true;
        }
    }
}