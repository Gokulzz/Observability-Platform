namespace TelemetryCollector.Domain.Models
{
    public class TelemetryEvent
    {
        public string ServiceName { get; set; } 
        public string EndPoint { get; set; }
        public string Method { get; set; }  
        public int StatusCode { get; set; } 
        public int ResponseTimeMs { get; set; } 
        public DateTime TimeStamp { get; set; } 
        public string CorrelationId { get; set; }   

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
