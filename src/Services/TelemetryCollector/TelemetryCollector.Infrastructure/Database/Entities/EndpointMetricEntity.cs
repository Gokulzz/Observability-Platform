namespace TelemetryCollector.Infrastructure.Database.Entities
{
    public class EndpointMetricEntity
    {
        public string ServiceName { get; set; } = default!;
        public string EndPoint { get; set; } = default!;
        public DateTime WindowStart { get; set; }

        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int ClientErrorRequests { get; set; }
        public int ServerErrorRequests { get; set; }

        public long TotalLatencyMs { get; set; }

    }

}
