using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Configures the <see cref="PartyType"/> entity for Entity Framework Core.
/// </summary>
public class PartyTypeConfiguration : IEntityTypeConfiguration<PartyType>
{
    /// <summary>
    /// Configures the <see cref="PartyType"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<PartyType> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(pt => pt.Description)
            .IsRequired()
            .HasMaxLength(512);
    }
}