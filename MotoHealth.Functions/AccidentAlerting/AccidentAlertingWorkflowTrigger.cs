using System;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using MotoHealth.Common;
using MotoHealth.Common.Dto;

namespace MotoHealth.Functions.AccidentAlerting
{
    public sealed class AccidentAlertingWorkflowTrigger
    {
        private readonly ILogger<AccidentAlertingWorkflowTrigger> _logger;
        private readonly IEventGridEventDataConverter _dataConverter;

        public AccidentAlertingWorkflowTrigger(
            ILogger<AccidentAlertingWorkflowTrigger> logger,
            IEventGridEventDataConverter dataConverter)
        {
            _logger = logger;
            _dataConverter = dataConverter;
        }
        
        [FunctionName(FunctionNames.AccidentAlerting.Trigger)]
        public async Task RunAsync(
            [EventGridTrigger] EventGridEvent gridEvent,
            [DurableClient] IDurableOrchestrationClient workflowStarter)
        {
            if (gridEvent.EventType != CommonConstants.EventTypes.AccidentAlerted)
            {
                _logger.LogError($"Got event {gridEvent.Id} of wrong type: {gridEvent.EventType}");
                
                return;
            }

            _logger.LogInformation($"Start handling {gridEvent.Id}");

            if (_dataConverter.TryConvertEventData<AccidentAlertEventDataDto>(gridEvent, out var eventData))
            {
                try
                {
                    await workflowStarter.StartNewAsync(FunctionNames.AccidentAlerting.Workflow, gridEvent.Id, eventData);
                    
                    _logger.LogInformation($"Successfully finished handling {gridEvent.Id}");
                }
                catch (InvalidOperationException exception)
                {
                    _logger.LogWarning(exception, "Ignored exception, while starting alerting workflow. Must have been duplicate");
                }
            }
            else
            {
                _logger.LogError($"Failed to handle {gridEvent.Id}. Event data is malformed");
            }
        }
    }
}