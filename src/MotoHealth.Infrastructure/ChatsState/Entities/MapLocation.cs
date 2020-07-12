using MotoHealth.Core.Bot.Abstractions;

namespace MotoHealth.Infrastructure.ChatsState.Entities
{
    internal sealed class MapLocation : IMapLocation
    {
        public double Longitude { get; set; }
        
        public double Latitude { get; set; }
    }
}