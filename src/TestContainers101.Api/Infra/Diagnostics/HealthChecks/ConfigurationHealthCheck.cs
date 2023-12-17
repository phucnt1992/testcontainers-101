using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TestContainers101.Api.Infra.Diagnostics.HealthChecks;

public class ConfigurationHealthCheck(IConfiguration configuration) : IHealthCheck
{
    private readonly IConfiguration _configuration = configuration;

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_configuration.GetConnectionString("Db")))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Missing connection string for Db"));
        }
        else if (string.IsNullOrWhiteSpace(_configuration.GetConnectionString("Cache")))
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Missing connection string for Cache"));
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
