using TelemetryCollector.Application.Services;

namespace TelemetryCollector.Api.Configurations
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITelemetryProcessingService, TelemetryProcessingService>();
            return services;
        }   
    }
}
