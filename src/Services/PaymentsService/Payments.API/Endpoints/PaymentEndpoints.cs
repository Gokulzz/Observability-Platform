using BuildingBlocks.Observability.ApiClient;
using BuildingBlocks.Observability.ApiClient.DTO;
using System.Diagnostics;

namespace Payments.API.Endpoints
{
    public  static class PaymentEndpoints
    {
        public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/payments", async (TelemetryClient telemetryClient) =>
            {
                var sw = Stopwatch.StartNew();
                await Task.Delay(Random.Shared.Next(50, 300));
                var statusCode = Random.Shared.Next(0, 10) < 8 ? 200 : 500;
                sw.Stop();
                await telemetryClient.SendTelemetryAsync(new TelemetryEventDTO(
                     "Payments",
                     "/api/payments",
                     "POST",
                     statusCode,
                     sw.ElapsedMilliseconds,
                     DateTime.UtcNow
               ));
            })
                .WithName("ProcessPayment")
                .WithTags("Payments");
            return endpoints;
        }
    }
}
