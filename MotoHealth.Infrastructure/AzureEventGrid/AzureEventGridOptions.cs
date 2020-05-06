namespace MotoHealth.Infrastructure.AzureEventGrid
{
    public sealed class AzureEventGridOptions
    {
        public string TopicEndpoint { get; set; } = string.Empty;

        public string TopicKey { get; set; } = string.Empty;
    }
}