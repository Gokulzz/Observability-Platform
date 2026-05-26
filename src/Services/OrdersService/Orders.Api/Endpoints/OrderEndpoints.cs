using BuildingBlocks.Observability.ApiClient;
using BuildingBlocks.Observability.ApiClient.DTO;
using System.Diagnostics;

namespace Orders.Api.Endpoints
{
    public static class OrderEndpoints
    {
        public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/orders", async (TelemetryClient telemetryClient) =>
            {
                var sw = Stopwatch.StartNew();
                await Task.Delay(Random.Shared.Next(50, 300));
                var statusCode = Random.Shared.Next(0, 10) < 8 ? 200 : 500;
                sw.Stop();
                await telemetryClient.SendTelemetryAsync(new TelemetryEventDTO(
                     "Orders",
                     "/api/orders",
                     "POST",
                     statusCode,
                     sw.ElapsedMilliseconds,
                     DateTime.UtcNow

               ));
                return Results.StatusCode(statusCode);

            })
                .WithName("CreateOrder")
                .WithTags("Orders");   
            return endpoints;
        }
    }
}
