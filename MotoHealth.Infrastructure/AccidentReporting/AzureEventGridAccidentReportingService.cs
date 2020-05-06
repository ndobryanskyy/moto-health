﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Infrastructure.AzureEventGrid;
using MotoHealth.PubSub;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    internal sealed class AzureEventGridAccidentReportingService : IAccidentReportingService
    {
        private readonly ILogger<IAccidentReportingService> _logger;
        private readonly IAzureEventGridPublisher _publisher;
        private readonly IMapper _mapper;

        public AzureEventGridAccidentReportingService(
            ILogger<IAccidentReportingService> logger,
            IAzureEventGridPublisher publisher,
            IMapper mapper)
        {
            _logger = logger;
            _publisher = publisher;
            _mapper = mapper;
        }

        public async Task ReportAccidentAsync(AccidentReport report, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Publishing report dated {report.ReportedAtUtc:g} from {report.DialogReferenceId}");

            var eventGridEvent = new EventGridEvent
            {
                Id = report.DialogReferenceId,
                Subject = report.DialogReferenceId,
                EventType = EventTypes.AccidentReported,
                EventTime = DateTime.UtcNow,
                Data = _mapper.Map<AccidentReportedEventData>(report),
                DataVersion = AccidentReportedEventData.Version
            };

            await _publisher.PublishEventAsync(eventGridEvent, cancellationToken);

            _logger.LogInformation($"Report dated {report.ReportedAtUtc:g} from {report.DialogReferenceId} dialog was published successfully");
        }
    }
}