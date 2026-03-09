using Microsoft.EntityFrameworkCore;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Domain.Models;
using TelemetryCollector.Infrastructure.Database.Entities;

namespace TelemetryCollector.Infrastructure.Database.Repostories
{
    public class EndpointMetricRepository : IEndpointMetricRepository
    {
        private readonly TelemetryDbContext _dbContext; 
        public EndpointMetricRepository(TelemetryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<EndpointMetric?> GetAsync(string serviceName, string endpoint, DateTime windowStart)
        {
            var result = await _dbContext.EndpointMetrics
                .AsNoTracking()
                .Where(em => em.ServiceName == serviceName &&
                             em.EndPoint == endpoint &&
                             em.WindowStart == windowStart)
                .FirstOrDefaultAsync();

            if (result == null)
                return null;

            return EndpointMetric.LoadEndpointMetric(
                result.ServiceName,
                result.EndPoint,
                result.WindowStart,
                result.TotalRequests,
                result.SuccessfulRequests,
                result.ClientErrorRequests,
                result.ServerErrorRequests,
                result.TotalLatencyMs
            );
        }


        public async Task SaveAsync(EndpointMetric metric)
        {
            var entity = await _dbContext.EndpointMetrics
                .FirstOrDefaultAsync(x =>
                    x.ServiceName == metric.ServiceName &&
                    x.EndPoint == metric.EndPoint &&
                    x.WindowStart == metric.TimeWindowStart);

            if (entity is null)
            {
                entity = new EndpointMetricEntity
                {
                    ServiceName = metric.ServiceName,
                    EndPoint = metric.EndPoint,
                    WindowStart = metric.TimeWindowStart
                };

                _dbContext.EndpointMetrics.Add(entity);
            }

            // Persist raw accumulator state
            entity.TotalRequests = metric.TotalRequests;
            entity.SuccessfulRequests = metric.SuccessfulRequests;
            entity.ClientErrorRequests = metric.ClientErrorRequests;
            entity.ServerErrorRequests = metric.ServerErrorRequests;
            entity.TotalLatencyMs = metric.GetTotalLatencyMs();

            await _dbContext.SaveChangesAsync();
        }

    }
}
