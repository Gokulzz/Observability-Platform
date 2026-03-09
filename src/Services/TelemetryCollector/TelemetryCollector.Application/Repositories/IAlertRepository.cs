using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Repositories
{
    public interface IAlertRepository
    {
        public Task SaveAlertAsync(Alert alert);
        public Task<IReadOnlyList<Alert>> GetRecentAlertsAsync(DateTime sinceUtc);  

    }
}
