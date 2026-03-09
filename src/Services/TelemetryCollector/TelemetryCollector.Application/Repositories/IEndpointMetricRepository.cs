using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Repositories
{
    public interface IEndpointMetricRepository
    {
        public Task<EndpointMetric?> GetAsync(string serviceName, string endpoint, DateTime windowStart);
        public Task SaveAsync(EndpointMetric endpointMetric); 

    }
}
