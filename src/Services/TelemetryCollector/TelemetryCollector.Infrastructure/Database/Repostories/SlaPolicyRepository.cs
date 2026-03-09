using Microsoft.EntityFrameworkCore;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Domain.Models;
using TelemetryCollector.Infrastructure.Database.Entities;

namespace TelemetryCollector.Infrastructure.Database.Repostories
{
    public class SlaPolicyRepository : ISlaPolicyRepository
    {
        private readonly TelemetryDbContext _dbContext;
        public SlaPolicyRepository(TelemetryDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<SlaPolicy?> GetSlaPolicy(string serviceName, string endpoint)
        {
            var slaPolicy = await _dbContext.SlaPolicies.AsNoTracking()
                                                          .Where(p => p.ServiceName == serviceName && p.EndPoint == endpoint).FirstOrDefaultAsync();
            if (slaPolicy == null)
                return null;
            return new SlaPolicy(serviceName, endpoint, slaPolicy.MaxErrorRate, slaPolicy.MaxP95LatencyMs, slaPolicy.MaxConsecutiveHealthFailures);


        }
        public async Task<IReadOnlyList<SlaPolicy>> GetAllSlaPolicies()
        {
            var slaPolicies = await _dbContext.SlaPolicies.AsNoTracking().ToListAsync();
            return slaPolicies.Select(p => new SlaPolicy(p.ServiceName, p.EndPoint, p.MaxErrorRate, p.MaxP95LatencyMs, p.MaxConsecutiveHealthFailures)).ToList();
        }

        public async Task AddSlaPolicy(SlaPolicy policy)
        {
            var addSlaPolicy = new SlaPolicyEntity
            {
                ServiceName = policy.ServiceName,
                EndPoint = policy.EndPoint,
                MaxErrorRate = policy.MaxErrorRate,
                MaxP95LatencyMs = policy.MaxP95LatencyMs,
                MaxConsecutiveHealthFailures = policy.MaxConsecutiveHealthFailures
            };
            await _dbContext.SlaPolicies.AddAsync(addSlaPolicy);
            await _dbContext.SaveChangesAsync();
        }
    } 
}
