using TelemetryCollector.Application.Health;
using TelemetryCollector.Application.Notifications;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Application.Time;
using TelemetryCollector.Domain.Models;
using TelemetryCollector.Domain.Models.Enums;

namespace TelemetryCollector.Application.Services
{
    public class TelemetryProcessingService : ITelemetryProcessingService
    {
        private readonly IEndpointMetricRepository endpointMetricRepository;
        private readonly ISlaPolicyRepository slaPolicyRepository;
        private readonly IAlertRepository alertRepository;
        private readonly IAlertPublisher alertPublisher; 
        private readonly IHealthStatusProvider healthStatusProvider;
        public TelemetryProcessingService(
            IEndpointMetricRepository endpointMetricRepository,
            ISlaPolicyRepository slaPolicyRepository,
            IAlertRepository alertRepository,
            IAlertPublisher alertPublisher, 
            IHealthStatusProvider healthStatusProvider)
        {
            this.endpointMetricRepository = endpointMetricRepository;
            this.slaPolicyRepository = slaPolicyRepository;
            this.alertRepository = alertRepository;
            this.alertPublisher = alertPublisher;
            this.healthStatusProvider = healthStatusProvider;
        }
        public async Task ProcessTelemetryAsync(TelemetryEvent telemetryEvent)
        {
            // Determine the time window for the telemetry event
            var windowStart = TimeWindowCalculator.FloorToMinute(telemetryEvent.Timestamp);
            // Retrieve or create the endpoint metric for the time window
            var metric = await endpointMetricRepository.GetEndpointMetricAsync(
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
            await endpointMetricRepository.SaveEndpointMetricAsync(metric);
            var healthStatus = await healthStatusProvider.GetHealthStatusAsync(telemetryEvent.ServiceName, telemetryEvent.EndPoint);
            // Retrieve SLA policies for the service
            var slaPolicies = await slaPolicyRepository .GetSlaPolicyByServiceName(telemetryEvent.ServiceName);
            // Evaluate policies and generate alerts if violations are found
            foreach (var policy in slaPolicies)
            {
                var violations = policy.EvaluateSla(metric, healthStatus);

                foreach (var violation in violations)
                {
                    Alert alert = violation.Type switch
                    {
                        AlertType.HighLatency =>
                            Alert.HighLatency(
                                violation.ServiceName,
                                violation.EndPoint,
                                violation.ActualValue),

                        AlertType.HighErrorRate =>
                            Alert.HighErrorRate(
                                violation.ServiceName,
                                violation.EndPoint,
                                violation.ActualValue),

                        AlertType.ServiceDown =>
                            Alert.ServiceDown(
                                violation.ServiceName,
                                violation.EndPoint),

                        _ => throw new InvalidOperationException("Unknown alert type")
                    };
                    await alertRepository.SaveAlertAsync(alert);
                    await alertPublisher.PublishAlertAsync(alert);
                }
            }
        }

    }
}
