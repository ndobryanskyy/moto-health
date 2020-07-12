using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class RecordAccidentActivity
    {
        private readonly IAccidentRecordingService _accidentRecordingService;
        private readonly IMapper _mapper;
        private readonly ILogger<RecordAccidentActivity> _logger;

        public RecordAccidentActivity(
            ILogger<RecordAccidentActivity> logger,
            IAccidentRecordingService accidentRecordingService,
            IMapper mapper)
        {
            _accidentRecordingService = accidentRecordingService;
            _mapper = mapper;
            _logger = logger;
        }

        [FunctionName(Constants.FunctionNames.AccidentAlerting.RecordAccidentActivity)]
        public async Task RunAsync(
            [ActivityTrigger] RecordAccidentActivityInput input,
            CancellationToken cancellationToken)
        {
            var accident = _mapper.Map<AccidentTableEntity>(input);

            await _accidentRecordingService.RecordAsync(accident, cancellationToken);

            _logger.LogInformation($"Successfully recorded accident {input.AccidentReport.Id} from {input.AccidentReport.ReporterTelegramUserId}");
        }
    }
}