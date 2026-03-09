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
        private readonly IHealthStateStore healthStatusProvider;
        private readonly ITimeProvider timeProvider;    
        public TelemetryProcessingService(
            IEndpointMetricRepository endpointMetricRepository,
            ISlaPolicyRepository slaPolicyRepository,
            IAlertRepository alertRepository,
            IAlertPublisher alertPublisher, 
            IHealthStateStore healthStatusProvider,
            ITimeProvider timeProvider)
        {
            this.endpointMetricRepository = endpointMetricRepository;
            this.slaPolicyRepository = slaPolicyRepository;
            this.alertRepository = alertRepository;
            this.alertPublisher = alertPublisher;
            this.healthStatusProvider = healthStatusProvider;
            this.timeProvider = timeProvider;
        }
        public async Task ProcessTelemetryAsync(TelemetryEvent telemetryEvent)
        {
            // Determine the time window for the telemetry event
            var windowStart = TimeWindowCalculator.FloorToMinute(telemetryEvent.Timestamp);
            // Retrieve or create the endpoint metric for the time window
            var metric = await endpointMetricRepository.GetAsync(
                telemetryEvent.ServiceName,
                telemetryEvent.EndPoint,
                windowStart);

            if (metric == null)
            {
                metric = new EndpointMetric(
                    telemetryEvent.ServiceName,
                    telemetryEvent.EndPoint,
                    windowStart);
            }

            metric.AddEvent(telemetryEvent);
            await endpointMetricRepository.SaveAsync(metric);
            var healthStatus = await healthStatusProvider.GetHealthStatusAsync(telemetryEvent.ServiceName);
            // Retrieve SLA policy for the specific service name and endpoint
            var slaPolicy = await slaPolicyRepository.GetSlaPolicy(telemetryEvent.ServiceName, telemetryEvent.EndPoint);
            // Evaluate policies and generate alerts if violations are found
            if (slaPolicy is null)
                return;
            var violations = slaPolicy.EvaluateSla(metric, healthStatus);
            foreach (var violation in violations)
            {
                var alert= violation.ToAlert(timeProvider.UtcNow);
                await alertRepository.SaveAlertAsync(alert);
                await alertPublisher.PublishAlertAsync(alert);
            }
            
        }

    }
}
