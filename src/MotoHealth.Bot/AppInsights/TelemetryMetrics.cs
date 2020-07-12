using System.Collections;
using System.Collections.Generic;

namespace MotoHealth.Bot.AppInsights
{
    internal sealed class TelemetryMetrics : IEnumerable<KeyValuePair<string, double>>
    {
        private readonly Dictionary<string, double> _metrics = new Dictionary<string, double>();

        public void Add(string key, double value) => _metrics.Add(key, value);

        public bool TryAdd(string key, double value) => _metrics.TryAdd(key, value);

        public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
        {
            return _metrics.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_metrics).GetEnumerator();
        }

        public IDictionary<string, double> AsDictionary()
            => _metrics;
    }
}