using Newtonsoft.Json;

namespace MotoHealth.Common.Dto
{
    public sealed class AccidentAlertEventDataDto
    {
        public static string Version = "1";

        [JsonProperty(Required = Required.Always)]
        public long[] ChatsToNotify { get; set; } = default!;

        [JsonProperty(Required = Required.Always)]
        public AccidentReportDto Report { get; set; } = default!;
    }
}