namespace MicroTodo.Infra.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TestContainers101.Api.Entities;

class AttachmentConfiguration : BaseConfiguration<Attachment>
{
    public override void Configure(EntityTypeBuilder<Attachment> builder)
    {
        base.Configure(builder);

        builder.ToTable("attachment");

        builder.Property(x => x.Path)
            .HasColumnName("path")
            .IsRequired();

        builder.Property(x => x.TodoItemId)
            .HasColumnName("todo_item_id")
            .IsRequired();

        builder.HasOne(x => x.BelongToItem)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
