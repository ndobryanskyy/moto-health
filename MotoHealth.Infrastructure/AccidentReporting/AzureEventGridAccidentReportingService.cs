using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MotoHealth.Common;
using MotoHealth.Common.Dto;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Infrastructure.AzureEventGrid;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    internal sealed class AzureEventGridAccidentReportingService : IAccidentReportingService
    {
        private readonly ILogger<IAccidentReportingService> _logger;
        private readonly IChatSubscriptionsService _subscriptionsService;
        private readonly IMapper _mapper;
        private readonly IAppEventsTopicClient _appEventsTopic;

        public AzureEventGridAccidentReportingService(
            ILogger<AzureEventGridAccidentReportingService> logger,
            IChatSubscriptionsService subscriptionsService,
            IMapper mapper,
            IAppEventsTopicClient appEventsTopic)
        {
            _logger = logger;
            _subscriptionsService = subscriptionsService;
            _mapper = mapper;
            _appEventsTopic = appEventsTopic;
        }

        public async Task ReportAccidentAsync(AccidentReport report, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start publishing report {report.Id}");

            var subscriptions = await _subscriptionsService.GetTopicSubscriptionsAsync(
                CommonConstants.AccidentReporting.AlertsChatSubscriptionTopicName,
                cancellationToken
            );

            if (subscriptions.Count < 1)
            {
                throw new InvalidOperationException("Tried to report accident before any chats were subscribed");
            }

            _logger.LogInformation($"{subscriptions.Count} chats would be notified about report {report.Id}");

            var mappedReport = _mapper.Map<AccidentReportDto>(report);
            var alert = new AccidentAlertEventDataDto
            {
                ChatsToNotify = subscriptions.Select(x => x.ChatId).ToArray(),
                Report = mappedReport
            };

            await _appEventsTopic.PublishAccidentAlertAsync(alert, cancellationToken);

            _logger.LogInformation($"Successfully published report {report.Id}");
        }
    }
}