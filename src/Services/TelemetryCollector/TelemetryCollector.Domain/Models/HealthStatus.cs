namespace TelemetryCollector.Domain.Models
{
    public sealed record HealthStatus(
        string ServiceName,
        bool IsHealthy,
        int ConsecutiveFailures
    );
}
