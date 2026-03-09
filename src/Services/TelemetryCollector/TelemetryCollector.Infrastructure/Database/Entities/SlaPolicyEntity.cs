
namespace TelemetryCollector.Infrastructure.Database.Entities
{
    public class SlaPolicyEntity
    {
        public string ServiceName { get; set; } = default!;
        public string EndPoint { get; set; } = default!;
        public double MaxErrorRate { get; set; }
        public double MaxP95LatencyMs { get; set; }
        public int MaxConsecutiveHealthFailures { get; set; }
    }
}
