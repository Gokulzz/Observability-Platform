
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Services
{
    public interface ITelemetryIngestionService
    {
        public Task IngestAsync(TelemetryEvent telemetryEvent);
    }
}
