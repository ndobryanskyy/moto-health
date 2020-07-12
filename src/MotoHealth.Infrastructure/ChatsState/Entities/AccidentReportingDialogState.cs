using System;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.ChatsState.Entities
{
    internal sealed class AccidentReportingDialogState : IAccidentReportingDialogState
    {
        public string InstanceId { get; set; } = string.Empty;

        public int Version { get; set; }

        public int CurrentStep { get; set; }

        public string ReportId { get; set; } = string.Empty;

        public DateTimeOffset StartedAt { get; set; }

        public string? Address { get; set; }

        [IgnoreProperty]
        IMapLocation? IAccidentReportingDialogState.Location
        {
            get => Location;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                Location = new MapLocation
                {
                    Latitude = value.Latitude,
                    Longitude = value.Longitude
                };
            }
        }

        public MapLocation? Location { get; set; }

        public string Participant { get; set; } = string.Empty;

        public string Victims { get; set; } = string.Empty;

        public string ReporterPhoneNumber { get; set; } = string.Empty;
    }
}