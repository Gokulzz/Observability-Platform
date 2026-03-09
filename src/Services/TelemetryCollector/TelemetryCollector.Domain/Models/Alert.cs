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


        private Alert(string serviceName, string endPoint, AlertType type, string description, DateTime triggeredAt)
        {
            ServiceName = serviceName;
            EndPoint = endPoint;
            Type = type;
            Description = description;
            TriggeredAt = triggeredAt;
        }
        // Factory methods for creating different types of alerts
        public static Alert HighLatency(string serviceName, string endPoint,  double p95Latency, DateTime triggeredAt)
        {
            return new Alert(serviceName,
                             endPoint,
                             AlertType.HighLatency,
                             $"P95 latency {p95Latency}ms excedded threshold",
                             triggeredAt);
            
        }  
        
        public static Alert HighErrorRate(string serviceName, string endPoint, double errorRate, DateTime triggeredAt)
        {
            return new Alert(serviceName,
                             endPoint,
                             AlertType.HighErrorRate,
                             $"Error rate {errorRate}% exceeded threshold",
                             triggeredAt);

        }

        public static Alert ServiceDown(string serviceName, string endPoint, DateTime triggeredAt)
        {
            return new Alert(serviceName,
                             endPoint,
                             AlertType.ServiceDown,
                             $"Service endpoint is down or unresponsive",
                             triggeredAt);
        }

        //to load domain model from database entity
        public static Alert LoadFromDbEntity(string serviceName, string endPoint, AlertType type, string description, DateTime triggeredAt)
        {
            return new Alert(serviceName,
                             endPoint,
                             type,
                             description,
                             triggeredAt);

        }

    }
}
