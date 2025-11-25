using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Configures the <see cref="RoleMetadata"/> entity for Entity Framework Core.
/// </summary>
public class RoleMetadataConfiguration : IEntityTypeConfiguration<RoleMetadata>
{
    /// <summary>
    /// Configures the <see cref="RoleMetadata"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<RoleMetadata> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(rm => rm.Id);

        builder.Property(rm => rm.RoleId)
            .IsRequired();

        builder.Property(rm => rm.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(rm => rm.Value)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne<Role>()
               .WithMany()
               .HasForeignKey(rm => rm.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}