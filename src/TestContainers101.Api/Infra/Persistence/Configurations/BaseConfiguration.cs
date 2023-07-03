using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TestContainers101.Api.Entities;

namespace MicroTodo.Infra.Persistence.Configurations;

abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.CreatedOn)
            .HasColumnName("created_on")
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(x => x.ModifiedOn)
            .HasColumnName("modified_on")
            .HasDefaultValue(DateTime.UtcNow);

        builder.Property(x => x.Version)
            .HasColumnName("version")
            .IsConcurrencyToken();
    }
}
