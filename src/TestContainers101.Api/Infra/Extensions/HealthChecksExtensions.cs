using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using TestContainers101.Api.Infra.Persistence;

namespace TestContainers101.Api.Infra.Extensions;

public static class HealthChecksExtensions
{
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("db", tags: ["ready"]);

        return services;
    }

    public static IEndpointRouteBuilder MapDefaultHealthChecksGroup(this IEndpointRouteBuilder endpoints, string pathGroup)
    {
        var group = endpoints.MapGroup(pathGroup);

        group.MapHealthChecks("/ready");

        group.MapHealthChecks("/live", new HealthCheckOptions
        {
            Predicate = r => r.Tags.Contains("live"),
        });

        return endpoints;
    }
}
