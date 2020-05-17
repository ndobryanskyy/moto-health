using Newtonsoft.Json;

namespace MotoHealth.PubSub.EventData
{
    public sealed class MapLocation
    {
        [JsonProperty(Required = Required.Always)]
        public double Latitude { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double Longitude { get; set; }
    }
}