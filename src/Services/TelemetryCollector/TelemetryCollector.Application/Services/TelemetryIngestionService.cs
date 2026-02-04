using TelemetryCollector.Application.Messaging;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Services
{
    public  class TelemetryIngestionService : ITelemetryIngestionService    
    {
        private readonly ITelemetryEventQueue telemetryEventQueue;
        public TelemetryIngestionService(ITelemetryEventQueue telemetryEventQueue)
        {
            this.telemetryEventQueue = telemetryEventQueue;
        }   

        public async Task IngestAsync(TelemetryEvent telemetryEvent)
        {
            await telemetryEventQueue.EnqueueEventAsync(telemetryEvent);
 
        }

    }
}
