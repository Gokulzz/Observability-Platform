namespace TelemetryCollector.Infrastructure.Implementations.Health
{
    public class ServiceHealthMonitoringOptions
    {
        public int IntervalSeconds { get; set; }
        public List<MonitoredServiceOptions> Services { get; set; } = new List<MonitoredServiceOptions>();

    }
    public class MonitoredServiceOptions
    {
        public string BaseUrl { get; set; } = default!;
        public string ServiceName { get; set; }= default!;  
        public string HealthEndpoint { get; set; } = default!;  

    }   
}
