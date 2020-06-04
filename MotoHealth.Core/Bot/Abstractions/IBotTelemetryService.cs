using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotTelemetryService
    {
        void OnNewChatStarted();

        void OnUpdateMapped(IBotUpdate botUpdate);

        void OnUpdateSkipped();

        void OnChatIsStillLocked();

        void OnUpdateHandled(bool successfully);

        void OnMotoHealthInfoProvided();

        void OnNothingToSay();

        void OnChatSubscribedToAccidentAlerting();

        void OnChatUnsubscribedFromAccidentAlerting();

        IAccidentReportingTelemetryService GetTelemetryServiceForAccidentReporting(IAccidentReportDialogState dialogState);
    }
}