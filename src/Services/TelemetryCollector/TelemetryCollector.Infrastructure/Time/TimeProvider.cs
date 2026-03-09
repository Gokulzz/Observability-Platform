using TelemetryCollector.Application.Time;

namespace TelemetryCollector.Infrastructure.Time
{
    public class TimeProvider : ITimeProvider   
    {
       public DateTime UtcNow => DateTime.UtcNow;
    }
}
