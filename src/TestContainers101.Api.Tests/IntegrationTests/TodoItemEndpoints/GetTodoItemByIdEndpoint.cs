namespace TestContainers101.Api.Tests.IntegrationTests.TodoItemEndpoints;

using System.Net;
using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using TestContainers101.Api.Entities;
using TestContainers101.Api.Infra.Persistence;
using TestContainers101.Api.Tests.Fakers;
using TestContainers101.Api.Tests.Fixtures;

public class GetTodoItemByIdEndpoint : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
{
    protected readonly TestWebApplicationFactory<Program> factory;

    public GetTodoItemByIdEndpoint(TestWebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    public async Task InitializeAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        var todoItem = new TodoItemFaker().Generate();

        await dbContext.TodoItems.AddAsync(todoItem);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task ShouldReturnTodoItemDetail()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/todo-items/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var todoItem = await response.Content.ReadFromJsonAsync<TodoItem>();

        todoItem.Should().NotBeNull()
            .And.Match<TodoItem>(x => x.Id == 1);
    }

    [Fact]
    public async Task ShouldReturnNotFoundWithInvalidId()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/todo-items/99");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

    }
}
