using TelemetryCollector.Domain.Models.Enums;

namespace TelemetryCollector.Domain.Models
{
    public record SlaViolation(
        AlertType Type,
        string ServiceName,
        string EndPoint,
        double ActualValue,
        double ThresholdValue
    )
    {
        // Convert the SLA violation into an alert 
        public Alert ToAlert(DateTime triggeredAt)
        {
            return Type switch
            {
                AlertType.HighLatency =>
                    Alert.HighLatency(ServiceName, EndPoint, ActualValue, triggeredAt),

                AlertType.HighErrorRate =>
                    Alert.HighErrorRate(ServiceName, EndPoint, ActualValue, triggeredAt),

                AlertType.ServiceDown =>
                    Alert.ServiceDown(ServiceName, EndPoint, triggeredAt),

                _ => throw new InvalidOperationException("Unknown alert type")
            };
        }
    }
}
