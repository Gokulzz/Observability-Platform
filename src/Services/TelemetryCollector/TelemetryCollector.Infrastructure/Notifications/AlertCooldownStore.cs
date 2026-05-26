using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using TelemetryCollector.Application.Notifications;
using TelemetryCollector.Domain.Models;
using TelemetryCollector.Domain.Models.Enums;

namespace TelemetryCollector.Infrastructure.Notifications
{
    public class AlertCooldownStore : IAlertCooldownStore
    {
        private readonly ConcurrentDictionary<AlertKey, DateTime> nextAllowedUtc = new();
        private readonly TimeSpan cooldown;

        public AlertCooldownStore(IOptions<AlertNotificationOptions> options)
        {
            cooldown = TimeSpan.FromSeconds(options.Value.CooldownSeconds);
        }

        public bool TryAcquire(Alert alert)
        {
            var key = new AlertKey(alert.ServiceName, alert.EndPoint, alert.Type);
            var nextAllowed = alert.TriggeredAt.Add(cooldown);

            while (true)
            {
                if (!nextAllowedUtc.TryGetValue(key, out var currentNextAllowed))
                {
                    if (nextAllowedUtc.TryAdd(key, nextAllowed))
                    {
                        return true;
                    }

                    continue;
                }

                if (alert.TriggeredAt < currentNextAllowed)
                {
                    return false;
                }

                if (nextAllowedUtc.TryUpdate(key, nextAllowed, currentNextAllowed))
                {
                    return true;
                }
            }
        }

        public void Release(Alert alert)
        {
            var key = new AlertKey(alert.ServiceName, alert.EndPoint, alert.Type);
            var reservation = new KeyValuePair<AlertKey, DateTime>(key, alert.TriggeredAt.Add(cooldown));
            ((ICollection<KeyValuePair<AlertKey, DateTime>>)nextAllowedUtc).Remove(reservation);
        }

        private readonly record struct AlertKey(string ServiceName, string EndPoint, AlertType Type);
    }
}
