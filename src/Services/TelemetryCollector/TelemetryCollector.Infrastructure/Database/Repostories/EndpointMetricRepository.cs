using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using TelemetryCollector.Application;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Application.Time;
using TelemetryCollector.Domain.Models;
using TelemetryCollector.Infrastructure.DTO;

namespace TelemetryCollector.Infrastructure.Database.Repostories
{
    public class EndpointMetricRepository : IEndpointMetricRepository
    {
        private readonly string _connectionString;

        public EndpointMetricRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TelemetryDb")!;
        }

        public async Task<EndpointMetric> UpsertAsync(TelemetryEvent telemetryEvent)
        {
            using var connection = new SqlConnection(_connectionString);
            var windowStart = TimeWindowCalculator.FloorToMinute(telemetryEvent.Timestamp);
            var bucketUpperBoundMs =
       LatencyBucketCalculator.GetBucketUpperBound(telemetryEvent.ResponseTimeMs);

            var entity= await connection.QuerySingleAsync<EndpointMetricDTO>(
                "UpsertEndpointsMetric",
                new
                {
                    ServiceName = telemetryEvent.ServiceName,
                    EndPoint =telemetryEvent.EndPoint,
                    WindowStart = windowStart,
                    ResponseTimeMs = telemetryEvent.ResponseTimeMs,
                    StatusCode=telemetryEvent.StatusCode,
                    BucketUpperBoundMs = bucketUpperBoundMs
                },
                commandType: CommandType.StoredProcedure
            );
            return EndpointMetric.LoadEndpointMetric(
                           entity.ServiceName,
                           entity.EndPoint,
                           entity.WindowStart,
                           entity.TotalRequests,
                           entity.SuccessfulRequests,
                           entity.ClientErrorRequests,
                           entity.ServerErrorRequests,
                           entity.TotalLatencyMs,
                           entity.P95LatencyMs
   );
        }
    }
}
