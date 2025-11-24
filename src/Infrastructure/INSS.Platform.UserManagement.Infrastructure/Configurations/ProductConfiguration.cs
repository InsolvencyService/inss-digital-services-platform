using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Configures the <see cref="Product"/> entity for Entity Framework Core.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    /// <summary>
    /// Configures the schema needed for the <see cref="Product"/> entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(p => p.Url)
            .HasMaxLength(2083);
    }
}