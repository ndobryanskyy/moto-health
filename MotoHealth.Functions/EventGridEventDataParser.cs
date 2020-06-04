using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace MotoHealth.Functions
{
    public interface IEventGridEventDataParser
    {
        bool TryParseEventData<TEventData>(
            EventGridEvent eventGridEvent,
            [NotNullWhen(true)] out TEventData? parsedData) where TEventData: class;
    }

    internal sealed class EventGridEventDataParser : IEventGridEventDataParser
    {
        private readonly ILogger<EventGridEventDataParser> _logger;

        public EventGridEventDataParser(ILogger<EventGridEventDataParser> logger)
        {
            _logger = logger;
        }

        public bool TryParseEventData<TEventData>(EventGridEvent eventGridEvent, out TEventData? parsedData) where TEventData : class
        {
            parsedData = null;

            if (eventGridEvent.Data is JObject jObject)
            {
                try
                {
                    parsedData = jObject.ToObject<TEventData>();

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