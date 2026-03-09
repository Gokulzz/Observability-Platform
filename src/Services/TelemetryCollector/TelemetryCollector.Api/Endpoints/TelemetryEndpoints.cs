using TelemetryCollector.Api.DTO;
using TelemetryCollector.Application.Messaging;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Api.Endpoints
{
    public static class TelemetryEndpoints
    {
        public static IEndpointRouteBuilder MapTelemetryEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/telemetry", async (TelemetryRequest request, ITelemetryEventQueue telemetryEventQueue) =>
            {
                var telemetryEvent = new TelemetryEvent(
                    request.ServiceName,
                    request.Endpoint,
                    request.Method,
                    request.StatusCode,
                    request.ResponseTimeMs,
                    request.Timestamp,
                    request.CorrelationId);
                await telemetryEventQueue.EnqueueEventAsync(telemetryEvent);
                return Results.Accepted();

            })
                .WithName("IngestTelemetry")
                .WithTags("Telemetry");
            return endpoints;
        }   
    }
}
