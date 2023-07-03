namespace MicroTodo.Infra.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TestContainers101.Api.Entities;

class TodoItemConfiguration : BaseConfiguration<TodoItem>
{
    public override void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        base.Configure(builder);

        builder.ToTable("todo_item");

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(x => x.Note)
            .HasColumnName("note");

        builder.Property(x => x.IsComplete)
            .HasColumnName("is_complete")
            .HasColumnType("boolean");

        builder.HasMany(x => x.Attachments)
            .WithOne(x => x.BelongToItem)
            .HasForeignKey(x => x.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
