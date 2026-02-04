using TelemetryCollector.Domain.Models.Enums;

namespace TelemetryCollector.Domain.Models
{
    public sealed record SlaViolation(
        AlertType Type,
        string ServiceName,
        string EndPoint,
        double ActualValue,
        double ThresholdValue
    );
}
