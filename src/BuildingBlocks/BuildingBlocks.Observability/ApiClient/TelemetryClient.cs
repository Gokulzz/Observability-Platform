
using BuildingBlocks.Observability.ApiClient.DTO;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace BuildingBlocks.Observability.ApiClient
{
    public class TelemetryClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<TelemetryClient> logger;

        public TelemetryClient(IHttpClientFactory httpClientFactory, ILogger<TelemetryClient> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task SendTelemetryAsync(TelemetryEventDTO telemetryEvent, CancellationToken cancellationToken = default)
        {
            var client = httpClientFactory.CreateClient("TelemetryClient");
            try
            {
                using var response = await client.PostAsJsonAsync("/api/telemetry", telemetryEvent, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning(
                        "Telemetry collector rejected event for {ServiceName} {Endpoint} with HTTP {StatusCode}.",
                        telemetryEvent.ServiceName,
                        telemetryEvent.EndPoint,
                        (int)response.StatusCode);
                }
            }
            catch (Exception ex) when ((ex is HttpRequestException or TaskCanceledException) && !cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning(
                    ex,
                    "Telemetry delivery failed for {ServiceName} {Endpoint}. The application request will continue.",
                    telemetryEvent.ServiceName,
                    telemetryEvent.EndPoint);
            }
        }
    }
}
