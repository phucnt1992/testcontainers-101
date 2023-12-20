using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Testcontainers.PostgreSql;
using Testcontainers.Redis;


namespace TestContainers101.Function.Tests.Fixtures;
public class FunctionContainersFactory IAsyncLifetime : class
{
    private readonly List<IContainer> _containers = [];
    private readonly Image

    public new Task DisposeAsync()
    {
        return Task
            .WhenAll(_containers.Select(container => container.DisposeAsync().AsTask()))
            .ContinueWith(async task => await base.DisposeAsync());
    }

    public Task InitializeAsync()
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Test";

        // Disable Testcontainers logs if CI environment
        if (envName.Equals("CI", StringComparison.OrdinalIgnoreCase))
        {
            TestcontainersSettings.Logger = new NullLogger<ILoggerFactory>();
        }

        var image = new I

    }

    public TestWebApplicationFactory<TProgram> WithStorageContainer()
    {


        return this;
    }

    public async Task StartContainersAsync(CancellationToken cancellationToken = default)
    {

    }

    public new HttpClient CreateClient() => Instance.CreateClient();

    public async Task StopContainersAsync()
    {

    }
}
