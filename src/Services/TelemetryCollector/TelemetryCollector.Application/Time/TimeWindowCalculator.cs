namespace TelemetryCollector.Application.Time
{
    public static class TimeWindowCalculator
    {
        public static DateTime FloorToMinute(DateTime timeStamp)
        {
            // Ensure the timestamp is in UTC
            var utc = timeStamp.Kind== DateTimeKind.Utc ? timeStamp : timeStamp.ToUniversalTime();
            return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0, DateTimeKind.Utc);
        }
    }
}
