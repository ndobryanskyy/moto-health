namespace MotoHealth.Infrastructure.AzureEventGrid
{
    public sealed class AzureEventGridOptions
    {
        public string TopicEndpoint { get; set; } = default!;

        public string TopicKey { get; set; } = default!;
    }
}