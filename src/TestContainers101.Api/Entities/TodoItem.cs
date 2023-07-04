namespace TestContainers101.Api.Entities;

public class TodoItem : BaseEntity
{
    public required string Title { get; set; }
    public string? Note { get; set; }
    public bool IsComplete { get; set; }
}
