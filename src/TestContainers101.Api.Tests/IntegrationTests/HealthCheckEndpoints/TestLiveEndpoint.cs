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
        var client = _factory
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:Db", "Host=localhost;Port=5432;Database=test_db;Username=postgres;Password=postgres");
                builder.UseSetting("ConnectionStrings:Cache", "localhost:6379");
            }).CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

    }

    [Theory]
    [InlineData("ConnectionStrings:Db")]
    [InlineData("ConnectionStrings:Cache")]
    public async Task ShouldReturn503_WhenConnectionStringIsEmpty(string key)
    {
        // Arrange
        var client = _factory
            .WithWebHostBuilder(builder => builder.UseSetting(key, string.Empty))
            .CreateClient();

        // Act
        var response = await client.GetAsync("/_healthz/live");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }
}
