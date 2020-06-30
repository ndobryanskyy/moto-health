using Newtonsoft.Json;

namespace MotoHealth.Common.Dto
{
    public sealed class MapLocationDto
    {
        [JsonProperty(Required = Required.Always)]
        public double Latitude { get; set; }

        [JsonProperty(Required = Required.Always)]
        public double Longitude { get; set; }
    }
}