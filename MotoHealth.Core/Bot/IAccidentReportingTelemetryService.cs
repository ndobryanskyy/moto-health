namespace MotoHealth.Core.Bot
{
    public interface IAccidentReportingTelemetryService
    {
        void OnStarted();

        void OnNextStep();

        void OnUnexpectedReply();

        void OnPhoneNumberSharedAutomatically();

        void OnCancelled();

        void OnCompleted();
    }
}