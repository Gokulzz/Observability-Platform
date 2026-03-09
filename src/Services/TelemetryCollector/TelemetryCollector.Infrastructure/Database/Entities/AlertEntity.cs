namespace TelemetryCollector.Infrastructure.Database.Entities
{
    public class AlertEntity
    {
        public Guid Id { get; set; }    
        public string ServiceName { get; set; } = default!;
        public string EndPoint { get; set; } = default!;
        public string Type { get; set; } = default!;    
        public string Description { get; set; } = default!;
        public DateTime TriggeredAt { get; set;  }
    }
}
