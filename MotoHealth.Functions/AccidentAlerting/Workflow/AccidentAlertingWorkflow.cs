using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using MotoHealth.Functions.ChatSubscriptions;
using MotoHealth.PubSub.EventData;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public static class AccidentAlertingWorkflow
    {
        private static readonly RetryOptions GetChatSubscriptionsActivityRetryOptions = new RetryOptions(TimeSpan.FromSeconds(5), 10)
        {
            BackoffCoefficient = 2,
            MaxRetryInterval = TimeSpan.FromMinutes(1),
            RetryTimeout = TimeSpan.FromMinutes(15)
        };

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

        [FunctionName(FunctionNames.AccidentAlerting.Workflow)]
        public static async Task RunAsync([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger logger)
        {
            logger = context.CreateReplaySafeLogger(logger);

            var accidentReport = context.GetInput<AccidentReportedEventData>() 
                                 ?? throw new ArgumentException("Input can not be null", nameof(context));

            var subscriptions = await GetChatSubscriptionsAsync(context);

            var chatsAlerted = 0;

            foreach (var subscription in subscriptions)
            {
                var alertActivityInput = new AlertChatActivityInput
                {
                    ChatId = subscription.ChatId,
                    AccidentReport = accidentReport
                };

                var alertActivityOutput = await AlertChatAsync(context, alertActivityInput, logger);

                if (alertActivityOutput.AlertSent)
                {
                    chatsAlerted++;
                }
            }

            var anyChatAlerted = chatsAlerted > 0;
            
            if (anyChatAlerted)
            {
                logger.LogInformation($"{chatsAlerted}/{subscriptions.Length} chats were alerted about report {accidentReport.ReportId}");
            }
            else
            {
                logger.LogError($"No chats were alerted about report {accidentReport.ReportId}!");
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

        private static async Task<ChatSubscription[]> GetChatSubscriptionsAsync(IDurableOrchestrationContext context)
        {
            return await context.CallActivityWithRetryAsync<ChatSubscription[]>(
                FunctionNames.AccidentAlerting.GetChatSubscriptionsActivity,
                GetChatSubscriptionsActivityRetryOptions,
                null
            );
        }

        private static async Task<AlertChatActivityOutput> AlertChatAsync(
            IDurableOrchestrationContext context,
            AlertChatActivityInput input,
            ILogger logger)
        {
            try
            {
                var output = await context.CallActivityWithRetryAsync<AlertChatActivityOutput>(
                    FunctionNames.AccidentAlerting.AlertChatActivity,
                    AlertChatActivityRetryOptions,
                    input
                );

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
                    FunctionNames.AccidentAlerting.RecordAccidentActivity,
                    RecordAccidentActivityRetryOptions,
                    input
                );
            }
            catch (FunctionFailedException exception)
            {
                logger.LogError(exception, $"Failed to record accident {input.AccidentReport.ReportId}");
            }
        }
    }
}