

using TelemetryCollector.Domain.Models.Enums;

namespace TelemetryCollector.Domain.Models
{
    public class SlaPolicy
    {
        public string ServiceName { get; }
        public string EndPoint { get; }
        public double MaxErrorRate { get;  }  
        public double MaxP95LatencyMs { get; }

        public int MaxConsecutiveHealthFailures { get; } 
        public SlaPolicy(string serviceName, string endPoint,   double maxErrorRate, double maxP95LatencyMs, int maxConsecutiveHealthFailure)
        {
            ServiceName = serviceName;
            EndPoint = endPoint;
            MaxErrorRate = maxErrorRate;
            MaxP95LatencyMs = maxP95LatencyMs;
            MaxConsecutiveHealthFailures = maxConsecutiveHealthFailure;
        }   
       
        public IReadOnlyList<SlaViolation> EvaluateSla(EndpointMetric metric, HealthStatus? healthStatus)
        {
            var violations = new List<SlaViolation>();
            if(metric.ServiceName != ServiceName || metric.EndPoint != EndPoint)
            {
                return violations;
            }
            if(healthStatus is not null && healthStatus.IsHealthy== false &&
                healthStatus.ConsecutiveFailures >= MaxConsecutiveHealthFailures)
            {
                violations.Add(new SlaViolation(
                    AlertType.ServiceDown,
                    ServiceName,
                    EndPoint,
                    healthStatus.ConsecutiveFailures,
                    MaxConsecutiveHealthFailures
                ));
                //return early if service is down   
                return violations;  
            }
            if (metric.ServerErrorRate > MaxErrorRate)
            {
                violations.Add(new SlaViolation(
                    AlertType.HighErrorRate,
                    ServiceName,
                    EndPoint,
                    metric.ServerErrorRate,
                    MaxErrorRate
                ));
            }
            if (metric.P95LatencyMs > MaxP95LatencyMs)
            {
                violations.Add(new SlaViolation(
                    AlertType.HighLatency,
                    ServiceName,
                    EndPoint,
                    metric.P95LatencyMs,
                    MaxP95LatencyMs
                ));
            }
            return violations;
        }   

    }
}
