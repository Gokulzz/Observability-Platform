using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Health
{
    public interface IHealthStatusProvider
    {
        Task<HealthStatus?> GetHealthStatusAsync(string serviceName, string endPoint);  
    }
}
