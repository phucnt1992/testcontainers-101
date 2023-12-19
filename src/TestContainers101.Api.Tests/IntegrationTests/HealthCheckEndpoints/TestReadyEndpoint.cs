namespace TestContainers101.Api.Tests.IntegrationTests.HealthCheckEndpoints;

using System.Net;

using FluentAssertions;

using TestContainers101.Api.Tests.Fixtures;

public class TestReadyEndpoint(TestWebApplicationFactory<Program> factory)
    : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task ShouldReturn200()
    {
        try
        {
            // Arrange
            await _factory
                .WithDbContainer()
                .WithCacheContainer()
                .StartContainersAsync();

            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/_healthz/ready");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        finally
        {
            await _factory.StopContainersAsync();
        }
    }

    [Fact]
    public async Task ShouldReturn503_WhenDatabaseIsNotReady()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }
}
