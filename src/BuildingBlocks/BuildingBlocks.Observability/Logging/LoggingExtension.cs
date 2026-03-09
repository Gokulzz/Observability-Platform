

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BuildingBlocks.Observability.Logging
{
   public static class LoggingExtension
    {
       public static IHostBuilder AddLogging(this IHostBuilder hostBuilder)
        {
            return hostBuilder.UseSerilog((context, services, logger) =>
            {
                logger.ReadFrom.Configuration(context.Configuration)
                      .Enrich.FromLogContext();
            });
        }

        public static IApplicationBuilder UseObservabilityRequestLogging(this  IApplicationBuilder app)
        {

            app.UseSerilogRequestLogging();
            return app;
        }   
    }   
}
