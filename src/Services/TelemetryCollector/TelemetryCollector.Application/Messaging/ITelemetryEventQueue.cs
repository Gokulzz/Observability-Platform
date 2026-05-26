using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Messaging
{
    public interface ITelemetryEventQueue
    {
        public Task EnqueueEventAsync(TelemetryEvent telemetryEvent);
        bool TryEnqueueEvent(TelemetryEvent telemetryEvent);
        IAsyncEnumerable<TelemetryEvent> DequeueEvent(CancellationToken cancellationToken);
    }
}
