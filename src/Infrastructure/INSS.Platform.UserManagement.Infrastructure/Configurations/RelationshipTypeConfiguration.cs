using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Configures the <see cref="RelationshipType"/> entity for Entity Framework Core.
/// </summary>
public class RelationshipTypeConfiguration : IEntityTypeConfiguration<RelationshipType>
{
    /// <summary>
    /// Configures the <see cref="RelationshipType"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<RelationshipType> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(rt => rt.Description)
            .IsRequired()
            .HasMaxLength(512);
    }
}