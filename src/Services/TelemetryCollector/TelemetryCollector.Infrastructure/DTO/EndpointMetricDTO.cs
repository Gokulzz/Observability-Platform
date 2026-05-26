namespace TelemetryCollector.Infrastructure.DTO
{
    public sealed record EndpointMetricDTO
    {
        public string ServiceName { get; init; } = default!;
        public string EndPoint { get; init; } = default!;
        public DateTime WindowStart { get; init; }

        public int TotalRequests { get; init; }
        public int SuccessfulRequests { get; init; }
        public int ClientErrorRequests { get; init; }
        public int ServerErrorRequests { get; init; }

        public long TotalLatencyMs { get; init; }
        public int P95LatencyMs { get; init; }
    }
}