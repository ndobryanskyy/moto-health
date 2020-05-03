using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using MotoHealth.Core.Bot;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class AppInsightBotTelemetryService : IBotTelemetryService
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IBotUpdateAccessor _botUpdateAccessor;

        private readonly Lazy<RequestTelemetry> _lazyRequestTelemetry;
        
        private IBotUpdate? _botUpdate;

        public AppInsightBotTelemetryService(
            TelemetryClient telemetryClient,
            IHttpContextAccessor httpContextAccessor,
            IBotUpdateAccessor botUpdateAccessor)
        {
            _telemetryClient = telemetryClient;
            _botUpdateAccessor = botUpdateAccessor;

            _lazyRequestTelemetry = new Lazy<RequestTelemetry>(() =>
            {
                var requestTelemetry = httpContextAccessor.HttpContext?.Features.Get<RequestTelemetry>();

                if (requestTelemetry == null)
                {
                    throw new InvalidOperationException();
                }

                return requestTelemetry;
            });
        }

        private RequestTelemetry RequestTelemetry => _lazyRequestTelemetry.Value;

        private IBotUpdate? BotUpdate
        {
            get => _botUpdate;
            set
            {
                _botUpdate = value ?? throw new ArgumentNullException(nameof(value));
                _botUpdateAccessor.Set(value);
            }
        }

        public void OnUpdateMapped(IBotUpdate botUpdate)
        {
            BotUpdate = botUpdate;

            RequestTelemetry.Properties.Add(TelemetryProperties.WellKnown.UpdateType, botUpdate.GetUpdateTypeNameForTelemetry());
        }

        public void OnUpdateSkipped()
        {
            EnsureBotUpdateIsPresent();

            RequestTelemetry.Properties.Add(TelemetryProperties.WellKnown.UpdateHandlingResult, UpdateHandlingTelemetryResult.Skipped.ToString());

            var diagnosticProperties = BotUpdate!.ExtractDiagnosticProperties();

            _telemetryClient.TrackEvent("Update Skipped", diagnosticProperties.AsDictionary());
        }

        public void OnChatIsStillLocked()
        {
            EnsureBotUpdateIsPresent();

            RequestTelemetry.Properties.Add(TelemetryProperties.WellKnown.UpdateHandlingResult, UpdateHandlingTelemetryResult.Error.ToString());

            var diagnosticProperties = BotUpdate!.ExtractDiagnosticProperties();

            diagnosticProperties.Add("Action Taken", "'Try again' message sent");

            _telemetryClient.TrackEvent("Message Arrived Before Previous Update Handled", diagnosticProperties.AsDictionary());
        }

        public void OnNewChatStarted()
        {
            EnsureBotUpdateIsPresent();

            _telemetryClient.TrackEvent("New Chat");
        }

        public void OnUpdateHandled(bool successfully)
        {
            EnsureBotUpdateIsPresent();

            var result = successfully
                ? UpdateHandlingTelemetryResult.Success
                : UpdateHandlingTelemetryResult.Error;

            RequestTelemetry.Properties.TryAdd(TelemetryProperties.WellKnown.UpdateHandlingResult, result.ToString());
        }

        public void OnMotoHealthInfoProvided()
        {
            EnsureBotUpdateIsPresent();

            _telemetryClient.TrackEvent("Moto Health Info Provided");   
        }

        public void OnNothingToSay()
        {
            EnsureBotUpdateIsPresent();

            var diagnosticProperties = BotUpdate!.ExtractDiagnosticProperties();

            _telemetryClient.TrackEvent("Bot Has Nothing to Say", diagnosticProperties.AsDictionary());
        }

        public IAccidentReportingTelemetryService GetTelemetryServiceForAccidentReporting(
            IAccidentReportDialogState dialogState)
        {
            EnsureBotUpdateIsPresent();

            return new AppInsightAccidentReportingTelemetryService(dialogState, BotUpdate!, _telemetryClient);
        }

        private void EnsureBotUpdateIsPresent()
        {
            if (BotUpdate == null)
            {
                throw new InvalidOperationException("Bot update is not set");
            }
        }
    }
}