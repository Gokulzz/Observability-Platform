using TelemetryCollector.Application.Notifications;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Infrastructure.Notifications
{
    public class AlertPublisher : IAlertPublisher
    {
        public  Task PublishAlertAsync(Alert alert)
        {
            // In a real implementation, this method would publish the alert to a messaging system, send an email, or integrate with an incident management tool.
            // For demonstration purposes, we'll just write the alert to the console.
            Console.WriteLine($"ALERT: {alert.Type} violation detected for {alert.ServiceName} at {alert.EndPoint} on {alert.TriggeredAt}. Details: {alert.Description}");
            return Task.CompletedTask;

        }
    }
}
