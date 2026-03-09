namespace TelemetryCollector.Domain.Models
{
    public class EndpointMetric
    {
        public string ServiceName { get; }
        public string EndPoint { get; }
        public DateTime TimeWindowStart { get; }

        // Running counters (persisted)
        private int _totalRequests;
        private int _successfulRequests;
        private int _clientErrorRequests;
        private int _serverErrorRequests;

        // Running latency stats
        private long _totalLatencyMs;

        // For now we keep a bounded latency sample for percentile estimation
        // (Later replace with histogram / TDigest if needed)
        private readonly List<long> _latencySamples = new();
        private const int MaxSamples = 1024; // prevent unbounded growth

        public EndpointMetric(string serviceName, string endPoint, DateTime timeWindowStart)
        {
            ServiceName = serviceName;
            EndPoint = endPoint;
            TimeWindowStart = timeWindowStart;
        }

        // Domain Behavior — called for every telemetry event
        public void AddEvent(TelemetryEvent telemetryEvent)
        {
            _totalRequests++;

            if (telemetryEvent.isSuccessful())
                _successfulRequests++;
            else if (telemetryEvent.isClientError())
                _clientErrorRequests++;
            else if (telemetryEvent.isServerError())
                _serverErrorRequests++;

            _totalLatencyMs += telemetryEvent.ResponseTimeMs;

            // Maintain bounded sample for percentile calculation
            if (_latencySamples.Count < MaxSamples)
            {
                _latencySamples.Add(telemetryEvent.ResponseTimeMs);
            }
            else
            {
                // simple reservoir-style replacement
                var index = Random.Shared.Next(MaxSamples);
                _latencySamples[index] = telemetryEvent.ResponseTimeMs;
            }
        }

        // Read Models (derived state)
        public int TotalRequests => _totalRequests;
        public int SuccessfulRequests => _successfulRequests;
        public int ClientErrorRequests => _clientErrorRequests;
        public int ServerErrorRequests => _serverErrorRequests;

        public double ServerErrorRate =>
            _totalRequests == 0 ? 0 :
            (double)_serverErrorRequests / _totalRequests * 100;

        public double AverageLatencyMs =>
            _totalRequests == 0 ? 0 :
            (double)_totalLatencyMs / _totalRequests;

        public double P95LatencyMs
        {
            get
            {
                if (_latencySamples.Count == 0)
                    return 0;

                var sorted = _latencySamples.OrderBy(x => x).ToList();
                int index = (int)Math.Ceiling(sorted.Count * 0.95) - 1;
                return sorted[index];
            }
        }

        // Used by repository when loading from DB
        internal static EndpointMetric LoadEndpointMetric
        (
        string serviceName,
        string endPoint,
        DateTime timeWindowStart,
        int totalRequests,
        int successfulRequests,
        int clientErrorRequests,
        int serverErrorRequests,
        long totalLatencyMs,
        IEnumerable<long>? latencySamples = null
        )
        {
            var metric = new EndpointMetric(serviceName, endPoint, timeWindowStart);

            metric._totalRequests = totalRequests;
            metric._successfulRequests = successfulRequests;
            metric._clientErrorRequests = clientErrorRequests;
            metric._serverErrorRequests = serverErrorRequests;
            metric._totalLatencyMs = totalLatencyMs;

            if (latencySamples != null)
                metric._latencySamples.AddRange(latencySamples);

            return metric;
        }
        internal long GetTotalLatencyMs() => _totalLatencyMs;   

    }


}
