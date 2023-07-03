namespace TestContainers101.Api.Infra.Persistence;

using Microsoft.EntityFrameworkCore;

using MicroTodo.Infra.Persistence.Configurations;

using TestContainers101.Api.Entities;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
    public DbSet<Attachment> Attachments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TodoItemConfiguration());
        modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
    }
}
