using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoHealth.AccidentReporting;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Infrastructure.ServiceBus;

namespace MotoHealth.Infrastructure.AccidentReporting
{
    internal sealed class ServiceBusAccidentsQueue : IAccidentsQueue
    {
        private readonly IMapper _mapper;
        private readonly ILogger<IAccidentsQueue> _logger;
        private readonly IMessageSender _messageSender;

        public ServiceBusAccidentsQueue(
            IServiceBusClientsFactory clientsFactory,
            IMapper mapper,
            IOptions<AccidentsQueueOptions> options,
            ILogger<IAccidentsQueue> logger)
        {
            _mapper = mapper;
            _logger = logger;

            var connectionString = options.Value.ConnectionString;
            var connectionStringBuilder = new ServiceBusConnectionStringBuilder(connectionString);

            _messageSender = clientsFactory.CreateMessageSenderClient(connectionStringBuilder);
        }

        public async Task EnqueueReportAsync(AccidentReport report, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Adding report dated {report.ReportedAtUtc:g} from {report.DialogReferenceId} dialog to queue");

            var mapped = _mapper.Map<AccidentReportDto>(report);
            var messageBody = mapped.ToByteArray();

            var message = new Message(messageBody)
            {
                MessageId = report.DialogReferenceId
            };

            await _messageSender.SendAsync(message);

            _logger.LogInformation($"Successfully added report dated {report.ReportedAtUtc:g} from {report.DialogReferenceId} dialog to queue");
        }
    }
}