using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Notifications
{
    public interface IAlertCooldownStore
    {
        bool TryAcquire(Alert alert);
        void Release(Alert alert);
    }
}
