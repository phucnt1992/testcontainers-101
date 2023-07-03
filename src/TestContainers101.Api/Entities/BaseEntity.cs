namespace TestContainers101.Api.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    public Guid Version { get; set; } = new Guid();
}
