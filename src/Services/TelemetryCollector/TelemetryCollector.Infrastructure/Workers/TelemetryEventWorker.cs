using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelemetryCollector.Application.Messaging;
using TelemetryCollector.Application.Services;

namespace TelemetryCollector.Infrastructure.Workers
{
    public class TelemetryEventWorker : BackgroundService
    {
        private readonly ITelemetryEventQueue telemetryEventQueue;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<TelemetryEventWorker> logger;
        public TelemetryEventWorker(
            ITelemetryEventQueue telemetryEventQueue,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<TelemetryEventWorker> logger)
        {
            this.telemetryEventQueue = telemetryEventQueue;
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Parallel.ForEachAsync(telemetryEventQueue.DequeueEvent(cancellationToken), 
            new ParallelOptions
            {
                MaxDegreeOfParallelism= 8
            },
            async (telemetryEvent, ct) =>
            {
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var telemetryProcessingService = scope.ServiceProvider.GetRequiredService<ITelemetryProcessingService>();
                    await telemetryProcessingService.ProcessTelemetryAsync(telemetryEvent);
                }
                catch (Exception ex) when (!ct.IsCancellationRequested)
                {
                    logger.LogError(
                        ex,
                        "Failed to process telemetry for {ServiceName} {Endpoint}. The event was skipped.",
                        telemetryEvent.ServiceName,
                        telemetryEvent.EndPoint);
                }
            });
        }

    }
}
