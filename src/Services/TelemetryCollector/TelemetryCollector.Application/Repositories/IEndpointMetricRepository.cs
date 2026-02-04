using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Repositories
{
    public interface IEndpointMetricRepository
    {
        public Task<EndpointMetric?> GetEndpointMetricAsync(string serviceName, string endpoint, DateTime windowStart);
        public Task SaveEndpointMetricAsync(EndpointMetric endpointMetric); 

    }
}
