
using System.Collections.Concurrent;
using TelemetryCollector.Application.Health;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Infrastructure.Implementations.Health
{
    public class HealthStateStore : IHealthStateStore
    {
        private readonly ConcurrentDictionary<string, HealthStatus> _healthStore = new();
        private static string Key(string serviceName) => serviceName;


        public async Task<HealthStatus?> GetHealthStatusAsync(string serviceName)
        {
            _healthStore.TryGetValue(Key(serviceName), out var healthStatus);
            return await Task.FromResult(healthStatus);
        }
        public async Task SaveAsync(HealthStatus healthStatus)
        {
            _healthStore[Key(healthStatus.ServiceName)] = healthStatus;
            await Task.CompletedTask;
        }
    }
}
