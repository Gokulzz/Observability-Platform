

namespace TelemetryCollector.Application
{
    public static class LatencyBucketCalculator
    {
        private static readonly int[] Buckets =
        [
            50,
            100,
            250,
            500,
            1000,
            2500,
            5000,
            10000,
            30000,
            60000
        ];

        public static int GetBucketUpperBound(long responseTimeMs)
        {
            foreach (var bucket in Buckets)
            {
                if (responseTimeMs <= bucket)
                    return bucket;
            }

            return Buckets[^1];
        }
    }

}
