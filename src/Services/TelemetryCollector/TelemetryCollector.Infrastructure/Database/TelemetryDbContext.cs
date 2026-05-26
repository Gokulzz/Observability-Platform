using Microsoft.EntityFrameworkCore;
using TelemetryCollector.Infrastructure.Database.Entities;

namespace TelemetryCollector.Infrastructure.Database
{
    public class TelemetryDbContext : DbContext
    {
        public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : base(options)
        {
        }

        public DbSet<EndpointMetricEntity> EndpointMetrics => Set<EndpointMetricEntity>();
        public DbSet<AlertEntity> Alerts => Set<AlertEntity>();
        public DbSet<SlaPolicyEntity> SlaPolicies => Set<SlaPolicyEntity>();
        public DbSet<EndpointLatencyBucketEntity> EndpointLatencyBuckets => Set<EndpointLatencyBucketEntity>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureEndpointMetric(modelBuilder);
            ConfigureAlert(modelBuilder);
            ConfigureSlaPolicy(modelBuilder);
            ConfigureEndpointLantencyBucketEntity(modelBuilder);

        }
        private static void ConfigureEndpointMetric(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<EndpointMetricEntity>();
            // Composite key = defines the aggregation bucket identity
            entity.HasKey(x => new { x.ServiceName, x.EndPoint, x.WindowStart });

            entity.Property(x => x.ServiceName)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(x => x.EndPoint)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(x => x.WindowStart)
                  .IsRequired();

            // precision tuning
            entity.Property(x => x.TotalLatencyMs)
                  .HasPrecision(18, 3);


            // index for querying recent windows
            entity.HasIndex(x => x.WindowStart);
        }

        private static void ConfigureAlert(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<AlertEntity>();
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ServiceName)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(x => x.EndPoint)
                  .IsRequired()
                  .HasMaxLength(500);
            entity.Property(x => x.Type)
                  .IsRequired();
            entity.Property(x => x.Description)
                  .IsRequired()
                  .HasMaxLength(1000);
            entity.Property(x => x.TriggeredAt)
                  .IsRequired();
            // index for querying recent alerts
            entity.HasIndex(x => x.TriggeredAt);
        }

        private static void ConfigureSlaPolicy(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<SlaPolicyEntity>();
            //composite key on ServiceName + EndPoint since we want one policy per endpoint
            entity.HasKey(x => new { x.ServiceName, x.EndPoint });
            entity.Property(x => x.EndPoint)
                  .IsRequired()
                  .HasMaxLength(500);
            entity.Property(x => x.ServiceName)
                   .IsRequired()
                   .HasMaxLength(200);
            entity.Property(x => x.MaxP95LatencyMs)
                 .HasPrecision(18, 3);
            entity.Property(x => x.MaxErrorRate)
                .HasPrecision(5, 2);
            entity.Property(x => x.MaxConsecutiveHealthFailures)
                .IsRequired();


        }

        private static void ConfigureEndpointLantencyBucketEntity(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<EndpointLatencyBucketEntity>();
            // Composite key = defines the aggregation bucket identity
            entity.HasKey(x => new { x.ServiceName, x.EndPoint, x.WindowStart, x.BucketUpperBoundMs });
            entity.Property(x => x.ServiceName)
                  .IsRequired()
                  .HasMaxLength(200);
            entity.Property(x => x.EndPoint)
                  .IsRequired()
                  .HasMaxLength(500);
            entity.Property(x => x.WindowStart)
                  .IsRequired();
            entity.Property(x => x.BucketUpperBoundMs)
                  .IsRequired();
            entity.Property(x => x.RequestCount)
                  .IsRequired();
            entity.HasIndex(x => new
            {
                x.ServiceName,
                x.EndPoint,
                x.WindowStart,
            });


        }
    }
}
