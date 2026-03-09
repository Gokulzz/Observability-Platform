namespace TelemetryCollector.Api.DTO
{
    public  record SlaPolicyDTO(
        string ServiceName,
        string Endpoint,
        double MaxErrorRate,
        double MaxP95LatencyMs,
        int MaxConsecutiveHealthFailures);  
}
