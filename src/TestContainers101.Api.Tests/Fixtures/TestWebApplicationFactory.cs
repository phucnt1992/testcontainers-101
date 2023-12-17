using DotNet.Testcontainers.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using Testcontainers.PostgreSql;
using Testcontainers.Redis;

using TestContainers101.Api.Tests.Extensions;

namespace TestContainers101.Api.Tests.Fixtures;
public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
{
    private readonly List<IContainer> _containers = [];

    public WebApplicationFactory<TProgram> Instance { get; private set; } = default!;

    public new Task DisposeAsync()
    {
        return Task
            .WhenAll(_containers.Select(container => container.DisposeAsync().AsTask()))
            .ContinueWith(async task => await base.DisposeAsync());
    }

    public Task InitializeAsync()
    {
        Instance = WithWebHostBuilder(builder => builder.UseEnvironment("Test"));

        return Task.CompletedTask;
    }

    public TestWebApplicationFactory<TProgram> WithCacheContainer()
    {
        _containers.Add(new RedisBuilder()
            .WithName($"test_cache_{Guid.NewGuid()}")
            .WithImage("redis:alpine")
            .WithCleanUp(true)
            .Build());

        return this;
    }

    public TestWebApplicationFactory<TProgram> WithDbContainer()
    {
        _containers.Add(new PostgreSqlBuilder()
            .WithDatabase($"test_db_{Guid.NewGuid()}")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithImage("postgres:alpine")
            .WithCleanUp(true)
            .Build());

        return this;
    }

    public async Task StartContainersAsync()
    {
        // Do nothing if no containers
        if (_containers.Count == 0)
        {
            return;
        }

        // Start all containers
        await Task.WhenAll(_containers.Select(container => container.StartWithWaitAndRetryAsync()));

        // Update Settings for each container
        Instance = _containers.Aggregate(this as WebApplicationFactory<TProgram>, (current, container) => current.WithWebHostBuilder(builder =>
        {
            switch (container)
            {
                case PostgreSqlContainer dbContainer:
                    builder.UseSetting("ConnectionStrings:Db", dbContainer.GetConnectionString());
                    break;

                case RedisContainer cacheContainer:
                    builder.UseSetting("ConnectionStrings:Cache", cacheContainer.GetConnectionString());
                    break;
            }
        }));
    }

    public new HttpClient CreateClient() => Instance.CreateClient();
}
