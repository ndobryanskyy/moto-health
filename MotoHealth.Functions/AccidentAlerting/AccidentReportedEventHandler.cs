using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using MotoHealth.PubSub;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Functions.AccidentAlerting
{
    public sealed class AccidentReportedEventHandler
    {
        private readonly ILogger<AccidentReportedEventHandler> _logger;
        private readonly IEventGridEventDataParser _dataParser;

        public AccidentReportedEventHandler(
            ILogger<AccidentReportedEventHandler> logger,
            IEventGridEventDataParser dataParser)
        {
            _logger = logger;
            _dataParser = dataParser;
        }
        
        [FunctionName(FunctionNames.AccidentReportedEventHandler)]
        public async Task HandleEventAsync(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [DurableClient] IDurableOrchestrationClient workflowStarter)
        {
            if (eventGridEvent.EventType != EventTypes.AccidentReported)
            {
                _logger.LogError($"Got event {eventGridEvent.Id} of wrong type: {eventGridEvent.EventType}");
                return;
            }

            _logger.LogInformation($"Start handling {eventGridEvent.Id}");

            if (_dataParser.TryParseEventData<AccidentReportedEventData>(eventGridEvent, out var eventData))
            {
                try
                {
                    await workflowStarter.StartNewAsync(FunctionNames.AccidentAlerting.Workflow, eventGridEvent.Id, eventData);
                    _logger.LogInformation($"Successfully finished handling {eventGridEvent.Id}");
                }
                catch (InvalidOperationException exception)
                {
                    _logger.LogWarning(exception, "Ignored exception, while starting alerting workflow. Must have been duplicate");
                }
            }
            else
            {
                _logger.LogError($"Failed to handle {eventGridEvent.Id}");
            }
        }
    }
}