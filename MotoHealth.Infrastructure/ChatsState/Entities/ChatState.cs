using System;
using Microsoft.Azure.Cosmos.Table;
using MotoHealth.Core.Bot.Abstractions;
using MotoHealth.Core.Bot.AccidentReporting;

namespace MotoHealth.Infrastructure.ChatsState.Entities
{
    internal sealed class ChatState : IChatState
    {
        public long AssociatedChatId { get; set; }

        public bool UserSubscribed { get; set; }

        public bool UserBanned { get; set; }

        [IgnoreProperty]
        IAccidentReportingDialogState? IChatState.AccidentReportDialog => AccidentReportDialog;

        public AccidentReportingDialogState? AccidentReportDialog { get; set; }

        public IAccidentReportingDialogState StartAccidentReportingDialog(int version)
        {
            AccidentReportDialog = new AccidentReportingDialogState
            {
                InstanceId = Guid.NewGuid().ToString(),
                ReportId = Guid.NewGuid().ToString(),
                StartedAt = DateTimeOffset.UtcNow,
                Version = version,
                CurrentStep = 1
            };

            return AccidentReportDialog;
        }

        public void CompleteAccidentReportingDialog()
        {
            AccidentReportDialog = null;
        }

        public TableEntity ToTableEntity() 
            => new TableEntityAdapter<ChatState>(this, AssociatedChatId.ToString(), ChatsTableEntityTypes.State) { ETag = "*" };

        public sealed class AccidentReportingDialogState : IAccidentReportingDialogState
        {
            public string ReportId { get; set; } = string.Empty;

            public int Version { get; set; }

            public string InstanceId { get; set; } = string.Empty;

            public int CurrentStep { get; set; }

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

        public sealed class MapLocation : IMapLocation
        {
            public double Longitude { get; set; }

            public double Latitude { get; set; }
        }
    }
}