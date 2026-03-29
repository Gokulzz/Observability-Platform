using System;

public class TelemetryClient
{
    private readonly IHttpClientFactory httpClientFactory;
    public TelemetryClient(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }
    public async Task SendTelemetryAsync(TelemetryEventDTO telemetryEvent)
    {
        var client = httpClientFactory.CreateClient("TelemetryClient");
        await client.PostAsJsonAsync("/api/telemetry", telemetryEvent);
    }
}
