namespace TestContainers101.Api.Tests.Fixtures;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Polly;

using Testcontainers.PostgreSql;

using TestContainers101.Api.Infra.Persistence;
using TestContainers101.Api.Tests.Extensions;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly PostgreSqlContainer _container;

    public TestWebApplicationFactory()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase($"test_db_{Guid.NewGuid()}")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithExposedPort(new Random(Guid.NewGuid().GetHashCode()).Next(49152, 65535))
            .WithImage("postgres:alpine")
            .WithCleanUp(true)
            .Build();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services => services
                .RemoveDbContext<AppDbContext>()
                .AddPooledDbContextFactory<AppDbContext>(options =>
                    options.UseNpgsql(_container.GetConnectionString())));

        return base.CreateHost(builder);
    }

    public new async Task DisposeAsync()
    {
        await _container.DisposeAsync();
        await base.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        var policy = Policy
            .Handle<AggregateException>()
            .WaitAndRetryAsync(new[]
            {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(6)
            });

        await policy.ExecuteAsync(_container.StartAsync, default);
    }
}
