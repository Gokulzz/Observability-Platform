namespace TelemetryCollector.Domain.Models
{
    public sealed record HealthStatus(
        string ServiceName,
        string EndPoint,
        bool IsHealthy,
        int ConsecutiveFailures
    );
}
