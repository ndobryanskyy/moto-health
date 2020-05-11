using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using MotoHealth.PubSub;
using MotoHealth.PubSub.EventData;
using Newtonsoft.Json.Linq;

namespace MotoHealth.Functions.AccidentAlerting
{
    public sealed class AccidentReportedEventHandler
    {
        private readonly ILogger<AccidentReportedEventHandler> _logger;

        public AccidentReportedEventHandler(ILogger<AccidentReportedEventHandler> logger)
        {
            _logger = logger;
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

            if (TryParseEventData(eventGridEvent, out var eventData))
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

        private bool TryParseEventData(EventGridEvent eventGridEvent, [NotNullWhen(true)] out AccidentReportedEventData? eventData)
        {
            eventData = null;

            if (eventGridEvent.Data is JObject jObject)
            {
                try
                {
                    eventData = jObject.ToObject<AccidentReportedEventData>();

                    return true;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Failed to parse event data of {eventGridEvent.Id}");
                }
            }
            else
            {
                _logger.LogError($"Event data of {eventGridEvent.Id} is not of {nameof(JObject)} type");
            }

            return false;
        }
    }
}