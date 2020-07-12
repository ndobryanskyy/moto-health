using MotoHealth.Core.Bot.Updates.Abstractions;

namespace MotoHealth.Core.Bot.Updates
{
    internal sealed class LocationMessageBotUpdate : MessageBotUpdateBase, ILocationMessageBotUpdate
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }
}