namespace TestContainers101.Api.Tests.IntegrationTests.HealthCheckEndpoints;

using System.Net;

using FluentAssertions;

using TestContainers101.Api.Tests.Fixtures;

public class TestReadyEndpoint : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public TestReadyEndpoint(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ShouldReturn200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz/ready");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }
}
