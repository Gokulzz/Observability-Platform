namespace TelemetryCollector.Domain.Models
{
    public record TelemetryEvent
    (
         string ServiceName,
         string EndPoint,   
         string Method,
         int StatusCode,
         long ResponseTimeMs,
         DateTime Timestamp,
         string CorrelationId
    )
    {
        public bool isSuccessful()
        {
            return StatusCode >= 200 && StatusCode < 300;
        }   
        public bool isClientError()
        {
            return StatusCode >= 400 && StatusCode < 500;
        }   

        public bool isServerError()
        {
            return StatusCode >= 500;
        }   



    }
}
