using TelemetryCollector.Application.Health;
using TelemetryCollector.Application.Notifications;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Application.Time;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Application.Services
{
    public class TelemetryProcessingService : ITelemetryProcessingService
    {
        private readonly IEndpointMetricRepository endpointMetricRepository;
        private readonly ISlaPolicyRepository slaPolicyRepository;
        private readonly IAlertRepository alertRepository;
        private readonly IAlertPublisher alertPublisher; 
        private readonly IAlertCooldownStore alertCooldownStore;
        private readonly IHealthStateStore healthStatusProvider;
        private readonly ITimeProvider timeProvider;    
        public TelemetryProcessingService(
            IEndpointMetricRepository endpointMetricRepository,
            ISlaPolicyRepository slaPolicyRepository,
            IAlertRepository alertRepository,
            IAlertPublisher alertPublisher, 
            IAlertCooldownStore alertCooldownStore,
            IHealthStateStore healthStatusProvider,
            ITimeProvider timeProvider)
        {
            this.endpointMetricRepository = endpointMetricRepository;
            this.slaPolicyRepository = slaPolicyRepository;
            this.alertRepository = alertRepository;
            this.alertPublisher = alertPublisher;
            this.alertCooldownStore = alertCooldownStore;
            this.healthStatusProvider = healthStatusProvider;
            this.timeProvider = timeProvider;
        }
        public async Task ProcessTelemetryAsync(TelemetryEvent telemetryEvent)
        {
           
            var metric = await endpointMetricRepository.UpsertAsync(telemetryEvent);
            var healthStatus = await healthStatusProvider.GetHealthStatusAsync(telemetryEvent.ServiceName);
            // Retrieve SLA policy for the specific service name and endpoint
            var slaPolicy = await slaPolicyRepository.GetSlaPolicy(telemetryEvent.ServiceName, telemetryEvent.EndPoint);
            // Evaluate policies and generate alerts if violations are found
            if (slaPolicy is null)
                return;
            var violations = slaPolicy.EvaluateSla(metric, healthStatus);
            foreach (var violation in violations)
            {
                var alert = violation.ToAlert(timeProvider.UtcNow);
                if (!alertCooldownStore.TryAcquire(alert))
                    continue;

                try
                {
                    await alertRepository.SaveAlertAsync(alert);
                }
                catch
                {
                    alertCooldownStore.Release(alert);
                    throw;
                }

                await alertPublisher.PublishAlertAsync(alert);
            }
            
        }

    }
}
