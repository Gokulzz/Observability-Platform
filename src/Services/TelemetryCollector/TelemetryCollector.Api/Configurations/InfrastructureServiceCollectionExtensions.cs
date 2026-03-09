using Microsoft.EntityFrameworkCore;
using TelemetryCollector.Application.Health;
using TelemetryCollector.Application.Messaging;
using TelemetryCollector.Application.Notifications;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Application.Services;
using TelemetryCollector.Application.Time;
using TelemetryCollector.Infrastructure.Database;
using TelemetryCollector.Infrastructure.Database.Repostories;
using TelemetryCollector.Infrastructure.Implementations.Health;
using TelemetryCollector.Infrastructure.Implementations.Messaging;
using TelemetryCollector.Infrastructure.Notifications;
using TelemetryCollector.Infrastructure.Workers;
using TimeProvider = TelemetryCollector.Infrastructure.Time.TimeProvider;

namespace TelemetryCollector.Api.Configurations
{
    public  static class InfrastructureServiceCollectionExtensions 
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TelemetryDbContext>(options =>
           options.UseSqlServer(configuration.GetConnectionString("TelemetryDb")));

            services.AddSingleton<ITelemetryEventQueue, TelemetryEventQueue>();
            services.AddSingleton<IHealthStatusService,  HealthStatusService>();

            services.AddScoped<IEndpointMetricRepository, EndpointMetricRepository>();
            services.AddScoped<ISlaPolicyRepository, SlaPolicyRepository>();
            services.AddScoped<IAlertRepository, AlertRepository>();

            services.AddSingleton<IAlertPublisher, AlertPublisher>();
            services.AddSingleton<ITimeProvider, TimeProvider>();
            services.AddSingleton<IHealthStateStore, HealthStateStore>();

            services.AddHostedService<TelemetryEventWorker>();
            services.AddHostedService<HealthCheckWorker>(); 
            var serviceHealthConfig = configuration.GetSection("ServiceHealthMonitoring");
            services.Configure<ServiceHealthMonitoringOptions>(serviceHealthConfig);
            var monitoredServices = serviceHealthConfig.Get<ServiceHealthMonitoringOptions>();
            foreach (var service in monitoredServices!.Services)
            {
                services.AddHttpClient(service.ServiceName, client =>
                {
                    client.BaseAddress = new Uri(service.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(5);
                });
            }

            return services;
         
        }
    }
}
