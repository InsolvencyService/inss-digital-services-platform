using Microsoft.EntityFrameworkCore.Metadata.Builders;
using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Provides extension methods for configuring auditable entities in Entity Framework Core.
/// </summary>
public static class AuditableEntityConfigurationExtensions
{
    /// <summary>
    /// Configures the common auditable properties for entities derived from <see cref="AuditableEntity"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity, which must inherit from <see cref="AuditableEntity"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity type.</param>
    public static void ConfigureAuditableEntity<T>(this EntityTypeBuilder<T> builder) where T : AuditableEntity
    {
        builder.Property(e => e.Id)
            .IsRequired();
        
        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(e => e.ModifiedBy)
            .HasMaxLength(255);
    }
}
