using Microsoft.EntityFrameworkCore;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Domain.Models;
using TelemetryCollector.Domain.Models.Enums;
using TelemetryCollector.Infrastructure.Database.Entities;

namespace TelemetryCollector.Infrastructure.Database.Repostories
{
    public class AlertRepository : IAlertRepository 
    {
        private readonly TelemetryDbContext _dbContext; 
        public AlertRepository(TelemetryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<Alert>> GetRecentAlertsAsync(DateTime sinceUtc)
        {
            var recentAlerts= await _dbContext.Alerts
                                              .AsNoTracking()
                                              .Where(a => a.TriggeredAt >= sinceUtc).OrderByDescending(a=> a.TriggeredAt).ToListAsync();
            return recentAlerts.Select(a => Alert.LoadFromDbEntity(a.ServiceName, 
                                                                   a.EndPoint, 
                                                                   Enum.Parse<AlertType>(a.Type), 
                                                                   a.Description,
                                                                   a.TriggeredAt))
                                                                    .ToList();

        }

        public async Task SaveAlertAsync(Alert alert)
        {
           var entity = new AlertEntity
           {
               ServiceName = alert.ServiceName,
               EndPoint = alert.EndPoint,
               Type = alert.Type.ToString(),
               Description = alert.Description,
               TriggeredAt = alert.TriggeredAt
           };
            await _dbContext.Alerts.AddAsync(entity);   
            await _dbContext.SaveChangesAsync();

        }
    }
}
