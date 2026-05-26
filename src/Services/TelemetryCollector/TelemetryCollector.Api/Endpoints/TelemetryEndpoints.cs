using TelemetryCollector.Api.DTO;
using TelemetryCollector.Application.Messaging;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Api.Endpoints
{
    public static class TelemetryEndpoints
    {
        public static IEndpointRouteBuilder MapTelemetryEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/telemetry", (TelemetryRequest request, ITelemetryEventQueue telemetryEventQueue) =>
            {
                var telemetryEvent = new TelemetryEvent(
                    request.ServiceName,
                    request.Endpoint,
                    request.Method,
                    request.StatusCode,
                    request.ResponseTimeMs,
                    request.Timestamp,
                    request.CorrelationId);
                return telemetryEventQueue.TryEnqueueEvent(telemetryEvent)
                    ? Results.Accepted()
                    : Results.StatusCode(StatusCodes.Status503ServiceUnavailable);

            })
                .WithName("IngestTelemetry")
                .WithTags("Telemetry");
            return endpoints;
        }   
    }
}
