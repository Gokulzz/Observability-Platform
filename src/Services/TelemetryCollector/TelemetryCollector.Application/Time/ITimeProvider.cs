namespace TelemetryCollector.Application.Time
{
    public interface ITimeProvider
    {
        public DateTime UtcNow { get; }

    }
}
