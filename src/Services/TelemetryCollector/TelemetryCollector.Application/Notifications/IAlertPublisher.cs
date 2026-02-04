using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Notifications
{
    public interface IAlertPublisher
    {
        public Task PublishAlertAsync(Alert alert); 
    }
}
