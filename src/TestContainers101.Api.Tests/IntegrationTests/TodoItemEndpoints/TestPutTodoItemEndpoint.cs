using System.Net;
using System.Net.Http.Json;

using Bogus;

using FluentAssertions;

using TestContainers101.Api.Entities;
using TestContainers101.Api.Tests.Extensions;
using TestContainers101.Api.Tests.Fakers;
using TestContainers101.Api.Tests.Fixtures;

namespace TestContainers101.Api.Tests.IntegrationTests.TodoItemEndpoints;

class InvalidPutData : TheoryData<object>
{
    public InvalidPutData()
    {
        var faker = new Faker();

        Add(new
        {
            Title = string.Empty,
            Note = faker.Lorem.Paragraph(),
            IsComplete = faker.Random.Bool()
        });
        Add(new
        {
            Title = faker.Lorem.Sentence(),
            Note = faker.Lorem.Letter(1001)
        });
        Add(new
        {
            Title = string.Empty,
            Note = faker.Lorem.Letter(1001),
            IsComplete = "invalid-bool"
        });
        Add(new
        {
            Title = faker.Lorem.Sentence(),
            Note = faker.Lorem.Letter(100),
            IsComplete = "invalid-bool"
        });
    }
}

public class TestPutTodoItemEndpoint(TestWebApplicationFactory<Program> factory) : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
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
        await _factory.StopContainersAsync();
    }


    [Fact]
    public async Task ShouldReturnOkAndUpdatedTodoItemIfSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var faker = new Faker();
        var data = new
        {
            Title = faker.Lorem.Sentence(),
            Note = faker.Lorem.Paragraph(),
            IsComplete = faker.Random.Bool()
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/todo-items/1", data);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseData = await response.Content.ReadFromJsonAsync<TodoItem>();
        responseData.Should().NotBeNull();
        responseData.Should().BeEquivalentTo(data);

    }

    [Theory(DisplayName = "Should return 404 if invalid id")]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(99)]
    public async Task ShouldReturnNotFound_WhenTodoItemIdDoesNotExist(long todoItemId)
    {
        // Arrange
        var client = _factory.CreateClient();
        var faker = new Faker();
        var todoItem = new
        {
            Title = faker.Lorem.Sentence(),
            Note = faker.Lorem.Paragraph(),
            IsComplete = faker.Random.Bool()
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/todo-items/{todoItemId}", todoItem);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory(DisplayName = "Should return bad request with invalid data")]
    [ClassData(typeof(InvalidPutData))]
    public async Task ShouldReturnBadRequest_WhenTodoItemIsInvalid(object data)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync("/api/todo-items/1", data);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
