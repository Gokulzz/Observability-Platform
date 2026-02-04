using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Repositories
{
    public interface ISlaPolicyRepository
    {
        public Task<IReadOnlyList<SlaPolicy>> GetSlaPolicyByServiceName(string serviceName);
    }
}
