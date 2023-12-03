namespace TestContainers101.Api.Tests.IntegrationTests.HealthCheckEndpoints;

using System.Net;

using FluentAssertions;

using TestContainers101.Api.Tests.Extensions;
using TestContainers101.Api.Tests.Fixtures;

public class TestHealthCheckEndpoint : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public TestHealthCheckEndpoint(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _factory.EnsureDeletedAsync();
    }

    [Fact]
    public async Task ShouldReturn204()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }
}
