using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace BuildingBlocks.Observability.Correlation
{
    public  class CorrelationMiddleware
    {
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private readonly RequestDelegate _next; 
        public CorrelationMiddleware(RequestDelegate next)
        {
            _next = next;
        }   
        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }
            context.Request.Headers[CorrelationIdHeader] = correlationId;
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
      
          

    }
}
