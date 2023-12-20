using System.Net;

using FluentAssertions;

using TestContainers101.Function.Tests.Fixtures;

namespace TestContainers101.Function.Tests.IntegrationTests.HealthCheckEndpoints
{
    public class TestLiveEndpoint(FunctionContainerFactory factory) : IClassFixture<FunctionContainerFactory>
    {
        private readonly FunctionContainerFactory _factory = factory;

        [Fact]
        public async Task ShouldReturnHealthy()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/health/live");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync())
                .Should().Be("Healthy");
        }
    }
}
