using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="AddressType"/> entity.
/// </summary>
public class AddressTypeConfiguration : IEntityTypeConfiguration<AddressType>
{
    /// <summary>
    /// Configures the <see cref="AddressType"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<AddressType> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(512);
    }
}

