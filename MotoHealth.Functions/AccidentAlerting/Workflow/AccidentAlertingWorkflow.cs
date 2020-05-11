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
        private static readonly RetryOptions AlertChatActivityRetryOptions = new RetryOptions(TimeSpan.FromSeconds(20), 5)
        {
            BackoffCoefficient = 2,
            MaxRetryInterval = TimeSpan.FromMinutes(1)
        };

        private static readonly RetryOptions GetChatSubscriptionsActivityRetryOptions = new RetryOptions(TimeSpan.FromSeconds(5), 10)
        {
            BackoffCoefficient = 2,
            MaxRetryInterval = TimeSpan.FromMinutes(1),
            RetryTimeout = TimeSpan.FromMinutes(15)
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
                var alertOutput = await AlertChatAsync(context, subscription, accidentReport, logger);
                if (alertOutput.AlertSent)
                {
                    chatsAlerted++;
                }
            }

            if (chatsAlerted == 0)
            {
                logger.LogError($"No chats were alerted about report {accidentReport.ReportId}!");
                // TODO think of restarting again
            }
            else
            {
                logger.LogInformation($"{chatsAlerted}/{subscriptions.Length} chats were alerted about report {accidentReport.ReportId}");
            }
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
            ChatSubscription chatSubscription,
            AccidentReportedEventData accidentReport,
            ILogger logger)
        {
            var chatId = chatSubscription.ChatId;

            var input = new AlertChatActivityInput
            {
                ChatId = chatId,
                AccidentReport = accidentReport
            };

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
                logger.LogError(exception, $"Failed to alert chat {chatId}");

                return new AlertChatActivityOutput
                {
                    AlertSent = false
                };
            }
        }
    }
}