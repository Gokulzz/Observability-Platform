namespace TelemetryCollector.Domain.Models
{
    public  class EndpointMetric
    {
       public string ServiceName { get; }
       public string EndPoint { get; }  
       public DateTime TimeWindowStart { get; }
       private readonly List<TelemetryEvent> _events = new();   
       public EndpointMetric(string serviceName, string endPoint, DateTime timeWindowStart)
       {
            ServiceName = serviceName;
            EndPoint = endPoint;
            TimeWindowStart = timeWindowStart;
       }
       
       public void AddEvent(TelemetryEvent telemetryEvent)
       {
                _events.Add(telemetryEvent);
       }

       public int TotalRequests => _events.Count;    
       public int SuccessfulRequests => _events.Count(e => e.isSuccessful()); 
       public int ClientErrorRequests => _events.Count(e => e.isClientError());
       public int ServerErrorRequests => _events.Count(e => e.isServerError());  
       public double ServerErrorRate
        {
            get
            {
                if (TotalRequests == 0) return 0;
                return (double)ServerErrorRequests / TotalRequests * 100;
            }   
        }
        
       public double AverageLatencyMs
        {
            get
            {
                if (TotalRequests == 0) return 0;
                return _events.Average(e => e.ResponseTimeMs);
            }
        }  

        public double P95LatencyMs
        {
            get
            {
                if (TotalRequests == 0) return 0;
                //for now we will compute p95 by sorting latencies and picking the 95th percentile
                //later we can optimize this with a more efficient algorithm or existing libraries if needed
                var sortedLatencies = _events.Select(e => e.ResponseTimeMs).OrderBy(latency => latency).ToList();
                int index = (int)Math.Ceiling(0.95 * sortedLatencies.Count) - 1;    
                return sortedLatencies[index];
            }
        }
    }


}
