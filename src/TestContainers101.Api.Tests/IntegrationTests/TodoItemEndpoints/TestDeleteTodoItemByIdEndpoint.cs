namespace TestContainers101.Api.Tests.IntegrationTests.TodoItemEndpoints;

using System.Net;

using FluentAssertions;

using TestContainers101.Api.Tests.Extensions;
using TestContainers101.Api.Tests.Fakers;
using TestContainers101.Api.Tests.Fixtures;

public class TestDeleteTodoItemByIdEndpoint(TestWebApplicationFactory<Program> factory) : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory = factory.WithDbContainer();
    private readonly TodoItemFaker _faker = new();

    public async Task InitializeAsync()
    {
        await _factory.StartContainersAsync();

        var todoItems = _faker.Generate(1);

        await _factory.EnsureCreatedAndPopulateDataAsync(todoItems);
    }

    public async Task DisposeAsync()
    {
        await _factory.EnsureDeletedAsync();
    }

    [Fact]
    public async Task ShouldReturnNoContentIfSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/todo-items/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory(DisplayName = "Should return 404 if invalid id")]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(99)]
    public async Task ShouldReturnNotFoundIfInvalidId(long id)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/todo-items/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
