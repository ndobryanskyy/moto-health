using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Core.Bot.Abstractions
{
    public interface IBotTelemetryService
    {
        void OnNewChatStarted();

        void OnMessageFromBannedChat();

        void OnUpdateSkipped();

        void OnUpdateHandled();

        void OnUpdateHandlingFailed();

        void OnMotoHealthInfoProvided();

        void OnNothingToSay();

        void OnChatSubscribedToAccidentAlerting();

        void OnChatUnsubscribedFromAccidentAlerting();

        IAccidentReportingTelemetryService GetTelemetryServiceForAccidentReporting(IAccidentReportingDialogState dialogState);
    }
}