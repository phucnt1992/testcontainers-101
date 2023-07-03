namespace TestContainers101.Api.Models;

public record CreateTodoItemRequest
{
    public string Title { get; set; } = null!;
    public string? Note { get; set; }
    public bool IsComplete { get; set; }
}

