namespace TestContainers101.Api.Tests.IntegrationTests.TodoItemEndpoints;

using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using TestContainers101.Api.Entities;
using TestContainers101.Api.Tests.Extensions;
using TestContainers101.Api.Tests.Fakers;
using TestContainers101.Api.Tests.Fixtures;

public class TestGetTodoItemByIdEndpoint(TestWebApplicationFactory<Program> factory) : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
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
    public async Task ShouldReturnTodoItemDetail()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/todo-items/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todoItem = await response.Content.ReadFromJsonAsync<TodoItem>();

        todoItem.Should().NotBeNull()
            .And.Match<TodoItem>(x => x.Id == 1);
    }

    [Theory(DisplayName = "Should return 404 with invalid id")]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(99)]
    public async Task ShouldReturnNotFoundWithInvalidId(long todoItemId)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/todo-items/{todoItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
}
