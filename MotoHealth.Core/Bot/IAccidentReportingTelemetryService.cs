﻿namespace MotoHealth.Core.Bot
{
    public interface IAccidentReportingTelemetryService
    {
        void OnStarted();

        void OnNextStep();

        void OnUnexpectedReply();

        void OnReplyValidationFailed();

        void OnPhoneNumberSharedAutomatically();

        void OnLocationSentAutomatically();

        void OnCancelled();

        void OnCompleted();
    }
}