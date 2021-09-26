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

        void OnUserBanned(long userId);

        void OnUserUnbanned(long userId);

        IAccidentReportingTelemetryService GetTelemetryServiceForAccidentReporting(IAccidentReportingDialogState dialogState);
    }
}