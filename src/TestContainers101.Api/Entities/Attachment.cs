namespace TestContainers101.Api.Entities;

public class Attachment : BaseEntity
{
    public required string Path { get; set; }
    public required long TodoItemId { get; set; }
    public required TodoItem BelongToItem { get; set; }
}
