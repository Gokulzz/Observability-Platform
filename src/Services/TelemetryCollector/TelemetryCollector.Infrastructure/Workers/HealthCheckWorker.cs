using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TelemetryCollector.Application.Services;
using TelemetryCollector.Infrastructure.Implementations.Health;

namespace TelemetryCollector.Infrastructure.Workers
{
    public class HealthCheckWorker : BackgroundService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IHealthStatusService healthStatusService;
        private readonly ServiceHealthMonitoringOptions serviceHealthMonitoringOptions;
        private readonly ILogger<HealthCheckWorker> logger;
        public HealthCheckWorker(
            IHttpClientFactory httpClientFactory,
            IHealthStatusService healthStatusService,
            ILogger<HealthCheckWorker> logger,
            IOptions<ServiceHealthMonitoringOptions> options)
        {
            this.httpClientFactory = httpClientFactory;
            this.healthStatusService = healthStatusService;
            serviceHealthMonitoringOptions = options.Value;
            this.logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var service in serviceHealthMonitoringOptions.Services)
                {
                    await CheckHealthAsync(service, stoppingToken);
                }
                await Task.Delay(TimeSpan.FromSeconds(serviceHealthMonitoringOptions.IntervalSeconds), stoppingToken);
            }

        }
        private async Task CheckHealthAsync(MonitoredServiceOptions serviceOptions, CancellationToken cancellationToken)
        {
            logger.LogInformation("Checking health for {ServiceName} at {HealthEndpoint}", serviceOptions.ServiceName, serviceOptions.HealthEndpoint);
            bool isHealthy;
            try
            {
                var client = httpClientFactory.CreateClient(serviceOptions.ServiceName);
                var response = await client.GetAsync(serviceOptions.HealthEndpoint, cancellationToken);
                isHealthy = response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
               
                    logger.LogError(ex, "Health check failed for {ServiceName} at {HealthEndpoint}", serviceOptions.ServiceName, serviceOptions.HealthEndpoint);
                    isHealthy = false;
                
              
            }
            await healthStatusService.UpdateHealthStatusAsync(serviceOptions.ServiceName, isHealthy);
        }
    }
}
