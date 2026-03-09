using System.Threading.Channels;
using TelemetryCollector.Application.Messaging;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Infrastructure.Implementations.Messaging
{
    public class TelemetryEventQueue : ITelemetryEventQueue
    {
        //channel
        private readonly Channel<TelemetryEvent> _channel;
        public TelemetryEventQueue()
        {
            var options = new BoundedChannelOptions(capacity: 10000)
            {
                SingleReader = false,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            };
            _channel = Channel.CreateBounded<TelemetryEvent>(options);
        }
        public IAsyncEnumerable<TelemetryEvent> DequeueEvent(CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        public async Task EnqueueEventAsync(TelemetryEvent telemetryEvent)
        {
           await _channel.Writer.WriteAsync(telemetryEvent);    
        }
    }
}
