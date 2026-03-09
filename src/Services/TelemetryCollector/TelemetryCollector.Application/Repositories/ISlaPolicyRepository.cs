using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Repositories
{
    public interface ISlaPolicyRepository
    {
        public Task<SlaPolicy?> GetSlaPolicy(string serviceName, string endpoint);
        public Task  AddSlaPolicy(SlaPolicy policy);
        public Task<IReadOnlyList<SlaPolicy>> GetAllSlaPolicies();  
    }
}
