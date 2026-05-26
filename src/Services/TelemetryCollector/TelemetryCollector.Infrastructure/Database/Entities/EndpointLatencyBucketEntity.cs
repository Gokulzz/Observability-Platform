
namespace TelemetryCollector.Infrastructure.Database.Entities
{
    public class EndpointLatencyBucketEntity
    {
        public string ServiceName { get; set; } = default!;
        public string EndPoint { get; set; } = default!;
        public DateTime WindowStart { get; set; }

        public int BucketUpperBoundMs { get; set; }
        public int RequestCount { get; set; }
    }
}
