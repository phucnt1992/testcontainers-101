using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;


namespace TestContainers101.Function.Tests.Fixtures;
public class FunctionContainersFactory : IAsyncLifetime
{

    private readonly IFutureDockerImage _image;

    public FunctionContainersFactory()
    {
        _image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "src/TestContainers101.Function")
            .WithBuildArgument(
                "RESOURCE_REAPER_SESSION_ID"
                ResourceReaper.DefaultSessionId.ToString("D")
            )
            .Build();
    }

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

        await _image.CreateAsync();


    }

    public TestWebApplicationFactory<TProgram> WithStorageContainer()
    {
        new

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
