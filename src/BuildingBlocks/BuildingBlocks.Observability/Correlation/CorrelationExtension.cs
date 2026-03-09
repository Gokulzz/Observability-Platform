
using Microsoft.AspNetCore.Builder;

namespace BuildingBlocks.Observability.Correlation
{
    public static class CorrelationExtension
    {
        public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        {
            return app.UseMiddleware<CorrelationMiddleware>();
        }
    }
}
