﻿using System;
using System.Globalization;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Http;
using MotoHealth.Bot.Extensions;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;
using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class AppInsightBotTelemetryService : IBotTelemetryService
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppInsightBotTelemetryService(TelemetryClient telemetryClient, IHttpContextAccessor httpContextAccessor)
        {
            _telemetryClient = telemetryClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext HttpContext => _httpContextAccessor.HttpContext ?? throw new InvalidOperationException();

        private RequestTelemetry RequestTelemetry => HttpContext.GetRequestTelemetry();

        private IBotUpdate BotUpdate => HttpContext.GetBotUpdate();

        public void OnNewChatStarted()
        {
            _telemetryClient.TrackEvent("New Chat");
        }

        public void OnUpdateSkipped()
        {
            RequestTelemetry.Properties[TelemetryProperties.WellKnown.UpdateHandlingResult] = UpdateHandlingTelemetryResult.Skipped.ToString();
            BotUpdate.ExtractDiagnosticProperties().MergeTo(RequestTelemetry.Properties);
        }

        public void OnUpdateHandled()
        {
            RequestTelemetry.Properties[TelemetryProperties.WellKnown.UpdateHandlingResult] = UpdateHandlingTelemetryResult.Success.ToString();
        }

        public void OnUpdateHandlingFailed()
        {
            RequestTelemetry.Properties[TelemetryProperties.WellKnown.UpdateHandlingResult] = UpdateHandlingTelemetryResult.Error.ToString();
            BotUpdate.ExtractDiagnosticProperties().MergeTo(RequestTelemetry.Properties);
        }

        public void OnMotoHealthInfoProvided()
        {
            _telemetryClient.TrackEvent("Moto Health Info Provided");   
        }

        public void OnNothingToSay()
        {
            var diagnosticProperties = BotUpdate.ExtractDiagnosticProperties();

            _telemetryClient.TrackEvent("Bot Has Nothing to Say", diagnosticProperties.AsDictionary());
        }

        public void OnChatSubscribedToAccidentAlerting()
        {
            _telemetryClient.TrackEvent("Subscribed to Accident Alerting");
        }

        public void OnChatUnsubscribedFromAccidentAlerting()
        {
            _telemetryClient.TrackEvent("Unsubscribed from Accident Alerting");
        }

        public void OnUserBanned(long userId)
        {
            var properties = new TelemetryProperties
            {
                { "Target User Id", userId.ToString(CultureInfo.InvariantCulture) }
            };

            _telemetryClient.TrackEvent("User Banned", properties.AsDictionary());
        }

        public void OnUserUnbanned(long userId)
        {
            var properties = new TelemetryProperties
            {
                { "Target User Id", userId.ToString(CultureInfo.InvariantCulture) }
            };

            _telemetryClient.TrackEvent("User unbanned", properties.AsDictionary());
        }

        public IAccidentReportingTelemetryService GetTelemetryServiceForAccidentReporting(IAccidentReportingDialogState dialogState) 
            => new AppInsightAccidentReportingTelemetryService(dialogState, BotUpdate, _telemetryClient);
    }
}