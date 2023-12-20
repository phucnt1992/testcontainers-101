using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Testcontainers.Azurite;


namespace TestContainers101.Function.Tests.Fixtures;
public class FunctionContainerFactory : IAsyncLifetime
{

    private readonly IFutureDockerImage _image;
    private AzuriteContainer _storageContainer = default!;
    private IContainer _functionContainer = default!;

    private const int FunctionPublicPort = 80;


    public FunctionContainerFactory()
    {
        _image = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), "src/TestContainers101.Function")
            .WithBuildArgument(
                "RESOURCE_REAPER_SESSION_ID",
                ResourceReaper.DefaultSessionId.ToString("D")
            )
            .WithDockerfile("Dockerfile.Testing")
            .WithCleanUp(true)
            .Build();
    }

    public Task DisposeAsync()
    {
        return Task.WhenAll(
            _functionContainer.DisposeAsync().AsTask(),
            _storageContainer.DisposeAsync().AsTask()
        )
        .ContinueWith(_ => _image.DisposeAsync());
    }

    public async Task InitializeAsync()
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Test";

        // Disable Testcontainers logs if CI environment
        if (envName.Equals("CI", StringComparison.OrdinalIgnoreCase))
        {
            TestcontainersSettings.Logger = new NullLogger<ILoggerFactory>();
        }

        await _image.CreateAsync();

        await WithStorageContainer()
            ._storageContainer.StartAsync();

        await WithFunctionContainer()
            ._functionContainer.StartAsync();

    }

    private FunctionContainerFactory WithStorageContainer()
    {
        _storageContainer = new AzuriteBuilder()
            .WithPortBinding(10000, true)
            .Build();

        return this;
    }

    private FunctionContainerFactory WithFunctionContainer()
    {
        _functionContainer = new ContainerBuilder()
            .WithImage(_image)
            .WithPortBinding(FunctionPublicPort, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(r => r.ForPort(FunctionPublicPort))
            )
            .WithEnvironment("AzureWebJobsStorage", _storageContainer.GetConnectionString())
            .DependsOn(_storageContainer)
            .Build();

        return this;
    }

    public HttpClient CreateClient()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(
                $"{Uri.UriSchemeHttp}://{_functionContainer.Hostname}:{_functionContainer.GetMappedPublicPort(FunctionPublicPort)}")
        };
        return client;
    }

}
