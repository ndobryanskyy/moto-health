using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public static class AccidentAlertingWorkflow
    {
        private static readonly RetryOptions AlertChatActivityRetryOptions = new RetryOptions(TimeSpan.FromSeconds(20), 5)
        {
            BackoffCoefficient = 2,
            MaxRetryInterval = TimeSpan.FromMinutes(1)
        };

        private static readonly RetryOptions RecordAccidentActivityRetryOptions = new RetryOptions(TimeSpan.FromSeconds(30), 5)
        {
            BackoffCoefficient = 2,
            MaxRetryInterval = TimeSpan.FromMinutes(5),
        };

        [FunctionName(Constants.FunctionNames.AccidentAlerting.Workflow)]
        public static async Task RunAsync(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger logger)
        {
            logger = context.CreateReplaySafeLogger(logger);

            var input = context.GetInput<AccidentAlertingWorkflowInput>() 
                                 ?? throw new ArgumentException("Input can not be null", nameof(context));

            var accidentReport = input.Report;

            var alertingTasks = input.ChatsToNotify.Select(chatId =>
            {
                var alertChatInput = new AlertChatActivityInput
                {
                    ChatId = chatId,
                    AccidentReport = accidentReport
                };

                return AlertChatAsync(context, alertChatInput, logger);
            });

            var alertingResults = await Task.WhenAll(alertingTasks);
            var chatsAlerted = alertingResults.Count(x => x.AlertSent);

            var anyChatAlerted = chatsAlerted > 0;
            
            if (anyChatAlerted)
            {
                logger.LogInformation($"{chatsAlerted}/{input.ChatsToNotify.Length} chats were alerted about report {accidentReport.Id}");
            }
            else
            {
                logger.LogError($"No chats were alerted about report {accidentReport.Id}!");
                // TODO think of restarting again
            }

            var recordActivityInput = new RecordAccidentActivityInput
            {
                AccidentReport = accidentReport,
                AnyChatAlerted = anyChatAlerted,
                ReportHandledAtUtc = context.CurrentUtcDateTime
            };

            await RecordAccidentAsync(context, recordActivityInput, logger);
        }

        private static async Task<AlertChatActivityOutput> AlertChatAsync(
            IDurableOrchestrationContext context,
            AlertChatActivityInput input,
            ILogger logger)
        {
            try
            {
                var output = await context.CallActivityWithRetryAsync<AlertChatActivityOutput>(
                    Constants.FunctionNames.AccidentAlerting.AlertChatActivity,
                    AlertChatActivityRetryOptions,
                    input);

                return output;
            }
            catch (FunctionFailedException exception)
            {
                logger.LogError(exception, $"Failed to alert chat {input.ChatId}");

                return new AlertChatActivityOutput
                {
                    AlertSent = false
                };
            }
        }

        private static async Task RecordAccidentAsync(
            IDurableOrchestrationContext context,
            RecordAccidentActivityInput input,
            ILogger logger)
        {
            try
            {
                await context.CallActivityWithRetryAsync(
                    Constants.FunctionNames.AccidentAlerting.RecordAccidentActivity,
                    RecordAccidentActivityRetryOptions,
                    input);
            }
            catch (FunctionFailedException exception)
            {
                logger.LogError(exception, $"Failed to record accident {input.AccidentReport.Id}");
            }
        }
    }
}