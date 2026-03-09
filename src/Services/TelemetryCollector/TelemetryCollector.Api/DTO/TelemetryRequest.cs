namespace TelemetryCollector.Api.DTO
{
    public record TelemetryRequest(
        string ServiceName,
        string Endpoint,
        string Method,
        int StatusCode,
        long ResponseTimeMs,
        DateTime Timestamp,
        string CorrelationId);
}
