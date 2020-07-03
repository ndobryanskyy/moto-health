using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MotoHealth.Common;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Events.Dto;
using MotoHealth.Infrastructure.AzureStorageQueue;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    internal sealed class AzureStorageQueueAccidentReportingService : IAccidentReportingService
    {
        private readonly ILogger<IAccidentReportingService> _logger;
        private readonly IChatSubscriptionsService _subscriptionsService;
        private readonly IMapper _mapper;
        private readonly IAppEventsQueueClient _queueClient;

        public AzureStorageQueueAccidentReportingService(
            ILogger<AzureStorageQueueAccidentReportingService> logger,
            IChatSubscriptionsService subscriptionsService,
            IMapper mapper,
            IAppEventsQueueClient queueClient)
        {
            _logger = logger;
            _subscriptionsService = subscriptionsService;
            _mapper = mapper;
            _queueClient = queueClient;
        }

        public async Task ReportAccidentAsync(AccidentReport report, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Start publishing report {report.Id}");

            var subscriptions = await _subscriptionsService.GetTopicSubscriptionsAsync(
                CommonConstants.AccidentReporting.AlertsChatSubscriptionTopicName,
                cancellationToken);

            if (subscriptions.Count < 1)
            {
                throw new InvalidOperationException("Tried to report accident before any chats were subscribed");
            }

            _logger.LogInformation($"{subscriptions.Count} chats will be notified about report {report.Id}");

            var alert = new AccidentAlertDto();

            var mappedReport = _mapper.Map<AccidentReportDto>(report);

            alert.ChatsToNotify.AddRange(subscriptions.Select(x => x.ChatId));
            alert.Report = mappedReport;

            await _queueClient.PublishAccidentAlertAsync(alert, cancellationToken);

            _logger.LogInformation($"Successfully published report {report.Id}");
        }
    }
}