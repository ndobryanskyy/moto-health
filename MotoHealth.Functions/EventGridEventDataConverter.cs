using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace MotoHealth.Functions
{
    public interface IEventGridEventDataConverter
    {
        bool TryConvertEventData<TEventData>(
            EventGridEvent eventGridEvent,
            [NotNullWhen(true)] out TEventData? eventData) where TEventData: class;
    }

    internal sealed class EventGridEventDataConverter : IEventGridEventDataConverter
    {
        private readonly ILogger<EventGridEventDataConverter> _logger;

        public EventGridEventDataConverter(ILogger<EventGridEventDataConverter> logger)
        {
            _logger = logger;
        }

        public bool TryConvertEventData<TEventData>(
            EventGridEvent eventGridEvent, 
            [NotNullWhen(true)] out TEventData? eventData) where TEventData : class
        {
            eventData = null;

            if (eventGridEvent.Data is JObject jObject)
            {
                try
                {
                    eventData = jObject.ToObject<TEventData>();

                    return true;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Failed to convert event data of {eventGridEvent.Id} to {typeof(TEventData).FullName}");
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