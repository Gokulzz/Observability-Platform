using TelemetryCollector.Api.DTO;
using TelemetryCollector.Application.Repositories;
using TelemetryCollector.Domain.Models;

namespace TelemetryCollector.Api.Endpoints
{
    public static class SlaPolicyEndpoint
    {
        public static IEndpointRouteBuilder MapSlaPolicyEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/sla-policies", async (ISlaPolicyRepository slaPolicyRepository) =>
            {
                var policies = await slaPolicyRepository.GetAllSlaPolicies();
                return Results.Ok(policies);
            })
            .WithName("GetSlaPolicies")
            .WithTags("SLA Policies");

            endpoints.MapPost("/api/sla-policies", async (SlaPolicyDTO policy, ISlaPolicyRepository slaPolicyRepository) =>
            {
                var addPolicy = new SlaPolicy(policy.ServiceName, policy.Endpoint, policy.MaxErrorRate, policy.MaxP95LatencyMs, policy.MaxConsecutiveHealthFailures);
                await slaPolicyRepository.AddSlaPolicy(addPolicy);
                return Results.CreatedAtRoute("GetSlaPolicy",new { serviceName = addPolicy.ServiceName, endpoint = addPolicy.EndPoint },addPolicy);

            })
                .WithName("AddSlaPolicy")
                .WithTags("SLA Policies");

            endpoints.MapGet("/api/sla-policies/{serviceName}/{endpoint}", async (string serviceName, string endpoint, ISlaPolicyRepository slaPolicyRepository) =>
            {
                var policy = await slaPolicyRepository.GetSlaPolicy(serviceName, endpoint);
                if (policy == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(policy);
            })
                .WithDescription("Get SLA policy for a specific service and endpoint")
                .WithName("GetSlaPolicy")
                .WithTags("SLA Policies");

            return endpoints;
        }
    }
}
