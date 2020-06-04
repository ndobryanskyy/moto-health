using Newtonsoft.Json;

namespace MotoHealth.PubSub.EventData
{
    public sealed class ChatTopicSubscriptionChangedEventData
    {
        public const string Version = "1";

        [JsonProperty(Required = Required.Always)]
        public long ChatId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Topic { get; set; } = string.Empty;
        
        [JsonProperty(Required = Required.Always)]
        public bool IsSubscribed { get; set; }
    }
}