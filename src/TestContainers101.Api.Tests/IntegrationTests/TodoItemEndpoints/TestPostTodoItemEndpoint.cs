namespace TestContainers101.Api.Tests.IntegrationTests.TodoItemEndpoints;

using System.Net;
using System.Net.Http.Json;

using Bogus;

using FluentAssertions;

using TestContainers101.Api.Tests.Extensions;
using TestContainers101.Api.Tests.Fixtures;


class InvalidPostData : TheoryData<object>
{
    public InvalidPostData()
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

public class TestPostTodoItemEndpoint(TestWebApplicationFactory<Program> factory) : IClassFixture<TestWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory = factory.WithDbContainer();

    public async Task InitializeAsync()
    {
        await _factory.StartContainersAsync();
        await _factory.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.EnsureDeletedAsync();
    }

    [Fact]
    public async Task ShouldReturnCreatedWithHeaderLocation()
    {
        // Arrange
        var client = _factory.CreateClient();
        var faker = new Faker();

        // Act
        var response = await client.PostAsJsonAsync("/api/todo-items", new
        {
            Title = faker.Lorem.Sentence(),
            Note = faker.Lorem.Paragraph(),
            IsComplete = faker.Random.Bool()
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Theory(DisplayName = "Should return bad request with invalid data")]
    [ClassData(typeof(InvalidPostData))]
    public async Task ShouldReturnBadRequestWithInvalidRequest(object data)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/todo-items", data);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
