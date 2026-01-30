using TelemetryCollector.Domain.Models.Enums;

namespace TelemetryCollector.Domain.Models
{
    public class Alert
    {
        public string ServiceName { get;  }
        public string EndPoint { get;  }
        public AlertType Type { get;  } 
        public string Description { get; } 
        public DateTime TriggeredAt { get;  } 

        private Alert(string serviceName, string endPoint, AlertType type, string description)
        {
            ServiceName = serviceName;
            EndPoint = endPoint;
            Type = type;
            Description = description;
            TriggeredAt = DateTime.UtcNow;
        }
        // Factory methods for creating different types of alerts
        public static Alert HighLatency(string serviceName, string endPoint,  double p95Latency)
        {
            return new Alert(serviceName,
                             endPoint,
                             AlertType.HighLatency,
                             $"P95 latency {p95Latency}ms excedded threshold");
            
        }  
        
        public static Alert HighErrorRate(string serviceName, string endPoint, double errorRate)
        {
            return new Alert(serviceName,
                             endPoint,
                             AlertType.HighErrorRate,
                             $"Error rate {errorRate}% excedded threshold");

        }

        public static Alert ServiceDown(string serviceName, string endPoint)
        {
            return new Alert(serviceName,
                             endPoint,
                             AlertType.ServiceDown,
                             $"Service endpoint is down or unresponsive");
        }
        
    }
}
