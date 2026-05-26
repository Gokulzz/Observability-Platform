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
        private double _p95LatencyMs;   

        public EndpointMetric(string serviceName, string endPoint, DateTime timeWindowStart)
        {
            ServiceName = serviceName;
            EndPoint = endPoint;
            TimeWindowStart = timeWindowStart;
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

        public double P95LatencyMs => _p95LatencyMs;

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
        double p95LatencyMs
        )
        {
            var metric = new EndpointMetric(serviceName, endPoint, timeWindowStart);

            metric._totalRequests = totalRequests;
            metric._successfulRequests = successfulRequests;
            metric._clientErrorRequests = clientErrorRequests;
            metric._serverErrorRequests = serverErrorRequests;
            metric._totalLatencyMs = totalLatencyMs;
            metric._p95LatencyMs = p95LatencyMs;    


            return metric;
        }
        internal long GetTotalLatencyMs() => _totalLatencyMs;   

    }


}
