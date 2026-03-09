using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Health
{
    public interface IHealthStateStore
    {
        Task<HealthStatus?> GetHealthStatusAsync(string serviceName);  
        Task SaveAsync(HealthStatus healthStatus);
    }
}
