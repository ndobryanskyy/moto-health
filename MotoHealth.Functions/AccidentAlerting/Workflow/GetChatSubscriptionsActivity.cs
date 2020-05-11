using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using MotoHealth.Functions.ChatSubscriptions;

namespace MotoHealth.Functions.AccidentAlerting.Workflow
{
    public sealed class GetChatSubscriptionsActivity
    {
        private readonly IAccidentAlertingSubscriptionsManager _accidentAlertingSubscriptionsManager;

        public GetChatSubscriptionsActivity(IAccidentAlertingSubscriptionsManager accidentAlertingSubscriptionsManager)
        {
            _accidentAlertingSubscriptionsManager = accidentAlertingSubscriptionsManager;
        }

        [FunctionName(FunctionNames.AccidentAlerting.GetChatSubscriptionsActivity)]
        public async Task<ChatSubscription[]> RunAsync([ActivityTrigger] IDurableActivityContext context)
        {
            return await _accidentAlertingSubscriptionsManager.GetSubscribedChatsAsync();
        }
    }
}