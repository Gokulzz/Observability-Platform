using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Repositories
{
    public interface IEndpointMetricRepository
    {
       public Task<EndpointMetric> UpsertAsync(TelemetryEvent telemetryEvent);  

    }
}
