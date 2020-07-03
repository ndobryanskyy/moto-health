using System;
using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Core.Bot.AccidentReporting
{
    public sealed class AccidentDetails
    {
        public AccidentDetails(
            string? address,
            IMapLocation? location,
            string participant,
            string victims)
        {
            if (address == null && location == null)
            {
                throw new ArgumentException($"{nameof(address)} and {nameof(location)} cannot be null at the same time");
            }

            Address = address;
            Location = location;
            Participant = participant;
            Victims = victims;
        }

        public string? Address { get; }

        public IMapLocation? Location { get; }

        public string Participant { get; }

        public string Victims { get; }
    }
}