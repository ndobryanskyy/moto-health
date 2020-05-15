using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class RecordAccidentActivity
    {
        private readonly IAccidentRecordingService _accidentRecordingService;

        public RecordAccidentActivity(IAccidentRecordingService accidentRecordingService)
        {
            _accidentRecordingService = accidentRecordingService;
        }

        [FunctionName(FunctionNames.AccidentAlerting.RecordAccidentActivity)]
        public async Task RunAsync([ActivityTrigger] RecordAccidentActivityInput input)
        {
            await _accidentRecordingService.RecordAsync(input);
        }
    }
}