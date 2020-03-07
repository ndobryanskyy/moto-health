using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace MotoHealth.Bot
{
    public static class Performance
    {
        private static readonly ConcurrentDictionary<string, Stopwatch> _stopwatches 
            = new ConcurrentDictionary<string, Stopwatch>();

        public static void StartMeasuring(string session)
        {
            _stopwatches.TryAdd(session, Stopwatch.StartNew());
        }

        public static void FinishMeasuring(string session)
        {
            if (_stopwatches.TryRemove(session, out var stopwatch))
            {
                stopwatch.Stop();

                Console.WriteLine($"Infrastructure overhead for: {session} - {stopwatch.ElapsedMilliseconds}");
            }
        }
    }
}