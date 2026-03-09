using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelemetryCollector.Application.Messaging;
using TelemetryCollector.Application.Services;

namespace TelemetryCollector.Infrastructure.Workers
{
    public class TelemetryEventWorker : BackgroundService
    {
        private readonly ITelemetryEventQueue telemetryEventQueue;
        private readonly IServiceScopeFactory serviceScopeFactory;
        public TelemetryEventWorker(
            ITelemetryEventQueue telemetryEventQueue,
            IServiceScopeFactory serviceScopeFactory )
        {
            this.telemetryEventQueue = telemetryEventQueue;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await foreach (var telemetryEvent in telemetryEventQueue.DequeueEvent(cancellationToken))
            {
                using var scope = serviceScopeFactory.CreateScope();
                var telemetryProcessingService = scope.ServiceProvider.GetRequiredService<ITelemetryProcessingService>();
                await telemetryProcessingService.ProcessTelemetryAsync(telemetryEvent);
            }
        }

    }
}
