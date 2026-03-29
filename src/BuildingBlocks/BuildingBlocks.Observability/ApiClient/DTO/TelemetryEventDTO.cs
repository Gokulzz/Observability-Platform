

namespace BuildingBlocks.Observability.ApiClient.DTO
{
    public record TelemetryEventDTO
   (
        string ServiceName,
        string EndPoint,
        string Method,
        int StatusCode,
        long ResponseTimeMs,
        DateTime Timestamp
   );
}
