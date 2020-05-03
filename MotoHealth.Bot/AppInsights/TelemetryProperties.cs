using System.Collections;
using System.Collections.Generic;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class TelemetryProperties : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _properties = new Dictionary<string, string>();

        public void Add(string key, string value) => _properties.Add(key, value);
        
        public bool TryAdd(string key, string value) => _properties.TryAdd(key, value);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _properties).GetEnumerator();
        }

        public IDictionary<string, string> AsDictionary() 
            => _properties;

        public static class WellKnown
        {
            public const string UpdateType = "Update Type";
            public const string OriginalUpdateType = "Original Update Type";
            public const string UpdateHandlingResult = "Update Handling Result";
            public const string Command = "Command";
            public const string MessageText = "Message Text";
        }
    }
}