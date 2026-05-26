using TelemetryCollector.Application.Notifications;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Infrastructure.Notifications
{
    public class AlertPublisher : IAlertPublisher
    {
        public  Task PublishAlertAsync(Alert alert)
        {
            Console.WriteLine($"ALERT: {alert.Type} violation detected for {alert.ServiceName} at {alert.EndPoint} on {alert.TriggeredAt}. Details: {alert.Description}");
            return Task.CompletedTask;

        }
    }
}
