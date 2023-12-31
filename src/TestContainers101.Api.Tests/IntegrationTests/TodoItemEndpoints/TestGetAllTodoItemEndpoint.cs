namespace TestContainers101.Api.Tests.IntegrationTests.TodoItemEndpoints;

using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using TestContainers101.Api.Entities;
using TestContainers101.Api.Tests.Extensions;
using TestContainers101.Api.Tests.Fakers;
using TestContainers101.Api.Tests.Fixtures;

public class TestGetAllTodoItemEndpoint(TestWebApplicationFactory<Program> factory) : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory = factory.WithDbContainer();
    private readonly TodoItemFaker _faker = new();

    public async Task InitializeAsync()
    {
        await _factory.StartContainersAsync();

        var todoItems = _faker.WithRandomIsComplete().Generate(10);
        await _factory.EnsureCreatedAndPopulateDataAsync(todoItems);
    }

    public async Task DisposeAsync()
    {
        await _factory.StopContainersAsync();
    }

    [Fact]
    public async Task ShouldReturnAllTodoItems()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/todo-items");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todoItems = await response.Content.ReadFromJsonAsync<IReadOnlyList<TodoItem>>();

        todoItems.Should().NotBeEmpty();
        todoItems.Should().HaveCount(10);
    }
}
