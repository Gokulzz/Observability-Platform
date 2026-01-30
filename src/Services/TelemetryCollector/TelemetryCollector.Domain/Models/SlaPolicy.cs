

namespace TelemetryCollector.Domain.Models
{
    public class SlaPolicy
    {
        public string ServiceName { get; }
        public string EndPoint { get; }
        public double MaxErrorRate { get;  }  
        public double MaxP95LatencyMs { get; }
        public SlaPolicy(string serviceName, string endPoint,   double maxErrorRate, double maxP95LatencyMs)
        {
            ServiceName = serviceName;
            EndPoint = endPoint;
            MaxErrorRate = maxErrorRate;
            MaxP95LatencyMs = maxP95LatencyMs;
        }   
        public bool IsViolation(EndpointMetric metric)
        {
            //check if the metric corresponds to this SLA policy    
            if (metric.ServiceName != ServiceName || metric.EndPoint != EndPoint)
                return false;
            return metric.ServerErrorRate > MaxErrorRate || metric.P95LatencyMs > MaxP95LatencyMs;
               
        }   

    }
}
