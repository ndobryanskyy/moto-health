using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotTelemetryService
    {
        void OnNewChatStarted();

        void OnUpdateSkipped();

        void OnUpdateHandled();

        void OnUpdateHandlingFailed();

        void OnMotoHealthInfoProvided();

        void OnNothingToSay();

        void OnChatSubscribedToAccidentAlerting();

        void OnChatUnsubscribedFromAccidentAlerting();

        void OnUserBanned(int userId);

        void OnUserUnbanned(int userId);

        IAccidentReportingTelemetryService GetTelemetryServiceForAccidentReporting(IAccidentReportingDialogState dialogState);
    }
}