
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Services
{
    public interface ITelemetryProcessingService
    {
        public Task ProcessTelemetryAsync(TelemetryEvent telemetryEvent);
    }
}
