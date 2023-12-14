namespace TestContainers101.Api.Tests.IntegrationTests.HealthCheckEndpoints;

using System.Net;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

public class TestLiveEndpoint : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TestLiveEndpoint(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ShouldReturn200()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }
}
