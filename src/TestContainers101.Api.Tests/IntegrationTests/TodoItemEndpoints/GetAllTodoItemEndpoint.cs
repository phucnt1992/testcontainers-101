namespace TestContainers101.Api.Tests.IntegrationTests.TodoItemEndpoints;

using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using TestContainers101.Api.Entities;
using TestContainers101.Api.Infra.Persistence;
using TestContainers101.Api.Tests.Fakers;
using TestContainers101.Api.Tests.Fixtures;

public class GetAllTodoItemEndpoint : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public GetAllTodoItemEndpoint(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var todoItems = new TodoItemFaker().Generate(10);

        await dbContext.TodoItems.AddRangeAsync(todoItems);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
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
