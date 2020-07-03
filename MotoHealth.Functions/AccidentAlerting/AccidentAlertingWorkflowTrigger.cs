using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MotoHealth.Common;
using MotoHealth.Events.Dto;
using MotoHealth.Functions.AccidentAlerting.Workflow;

namespace MotoHealth.Functions.AccidentAlerting
{
    public sealed class AccidentAlertingWorkflowTrigger
    {
        private readonly ILogger<AccidentAlertingWorkflowTrigger> _logger;
        private readonly IMapper _mapper;

        public AccidentAlertingWorkflowTrigger(
            ILogger<AccidentAlertingWorkflowTrigger> logger,
            IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }
        
        [FunctionName(Constants.FunctionNames.AccidentAlerting.Trigger)]
        public async Task RunAsync(
            [QueueTrigger(
                CommonConstants.AccidentReporting.AlertsQueueName, 
                Connection = Constants.AzureStorage.StorageAccountConnectionStringName)] byte[] data,
            [DurableClient] IDurableOrchestrationClient workflowStarter)
        {
            var alertDto = AccidentAlertDto.Parser.ParseFrom(data);
            var alertingWorkflowInput = _mapper.Map<AccidentAlertingWorkflowInput>(alertDto);

            var reportId = alertDto.Report.Id;

            _logger.LogInformation($"Start handling {reportId}");

            try
            {
                await workflowStarter.StartNewAsync(Constants.FunctionNames.AccidentAlerting.Workflow, reportId, alertingWorkflowInput);

                _logger.LogInformation($"Successfully triggered alerting workflow for {reportId}");
            }
            catch (InvalidOperationException exception)
            {
                _logger.LogWarning(exception, "Ignored exception, while starting alerting workflow. Must have been duplicate");
            }
        }
    }
}