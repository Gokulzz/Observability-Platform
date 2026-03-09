
using TelemetryCollector.Application.Health;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Services
{
    public class HealthStatusService : IHealthStatusService 
    {
        private readonly IHealthStateStore _healthStateStore;
        public  HealthStatusService(IHealthStateStore healthStateStore)
        {
            _healthStateStore = healthStateStore;
        }   
        public async Task UpdateHealthStatusAsync(string serviceName,  bool isHealthy)
        {
            var previousHealthStatus = await _healthStateStore.GetHealthStatusAsync(serviceName);  
            int failures = isHealthy ? 0 : (previousHealthStatus?.ConsecutiveFailures ?? 0) + 1;
            var newHealthStatus = new HealthStatus(serviceName, isHealthy , failures);
            await _healthStateStore.SaveAsync(newHealthStatus);
        }
            
    }
}
