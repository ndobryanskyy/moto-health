using System;
using Microsoft.ApplicationInsights;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class AppInsightAccidentReportingTelemetryService : IAccidentReportingTelemetryService
    {
        private const string DialogDurationInSecMetricsKey = "Dialog Duration, Sec";
        private const string StepsCompletedMetricsKey = "Steps Completed";

        private static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        private readonly IAccidentReportDialogState _dialogState;
        private readonly IBotUpdate _botUpdate;
        private readonly TelemetryClient _telemetryClient;

        public AppInsightAccidentReportingTelemetryService(
            IAccidentReportDialogState dialogState,
            IBotUpdate botUpdate,
            TelemetryClient telemetryClient)
        {
            _dialogState = dialogState;
            _botUpdate = botUpdate;
            _telemetryClient = telemetryClient;
        }

        public void OnStarted()
        {
            var properties = new TelemetryProperties
            {
                { "Started At", _dialogState.StartedAt.ToString("O") }
            };

            Track("Started", properties);
        }

        public void OnNextStep()
        {
            var properties = new TelemetryProperties
            {
                {"To Step", _dialogState.CurrentStep.ToString()}
            };

            Track("Advanced", properties);
        }

        public void OnUnexpectedReply()
        {
            var diagnosticProperties = _botUpdate.ExtractDiagnosticProperties();

            diagnosticProperties.Add("Failed Step", _dialogState.CurrentStep.ToString());

            Track("Unexpected Reply", diagnosticProperties);
        }

        public void OnPhoneNumberSharedAutomatically()
        {
            Track("Phone Was Shared Automatically");
        }

        public void OnCancelled()
        {
            var metrics = new TelemetryMetrics
            {
                {DialogDurationInSecMetricsKey, GetDialogDurationSec()},
                {StepsCompletedMetricsKey, _dialogState.CurrentStep - 1}
            };

            Track("Cancelled", metrics: metrics);
        }

        public void OnCompleted()
        {
            var metrics = new TelemetryMetrics
            {
                {DialogDurationInSecMetricsKey, GetDialogDurationSec()},
                {StepsCompletedMetricsKey, _dialogState.CurrentStep}
            };

            Track("Completed Successfully", metrics: metrics);
        }

        private void Track(
            string eventSuffix,
            TelemetryProperties? properties = null,
            TelemetryMetrics? metrics = null)
        {
            properties ??= new TelemetryProperties();

            properties.TryAdd("Dialog InstanceId", _dialogState.InstanceId);
            properties.TryAdd("Dialog Version", _dialogState.Version.ToString());

            _telemetryClient.TrackEvent($"AccidentReporting:{eventSuffix}", properties.AsDictionary(), metrics?.AsDictionary());
        }

        private double GetDialogDurationSec()
        {
            var timeSpan = DateTimeOffset.UtcNow - _dialogState.StartedAt;

            return timeSpan < OneDay
                ? timeSpan.TotalSeconds
                : OneDay.TotalSeconds + 1;
        }
    }
}