

namespace TelemetryCollector.Application.Services
{
    public interface IHealthStatusService
    {
        Task UpdateHealthStatusAsync(string serviceName,  bool isHealthy);
    }
}
