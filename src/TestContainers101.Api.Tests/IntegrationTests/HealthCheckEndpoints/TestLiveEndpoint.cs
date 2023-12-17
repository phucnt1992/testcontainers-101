namespace TestContainers101.Api.Tests.IntegrationTests.HealthCheckEndpoints;

using System.Net;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;

public class TestLiveEndpoint(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory = factory;

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

    [Fact]
    public async Task ShouldReturn503_WhenDbConnectionStringIsEmpty()
    {
        // Arrange
        var client = _factory
            .WithWebHostBuilder(builder => builder.UseSetting("ConnectionStrings:Db", string.Empty))
            .CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task ShouldReturn503_WhenCacheConnectionStringIsEmpty()
    {
        // Arrange
        var client = _factory
            .WithWebHostBuilder(builder => builder.UseSetting("ConnectionStrings:Cache", string.Empty))
            .CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }
}
