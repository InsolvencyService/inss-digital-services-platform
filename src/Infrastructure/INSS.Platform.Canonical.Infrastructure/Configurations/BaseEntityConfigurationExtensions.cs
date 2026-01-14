using INSS.Platform.Canonical.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.Canonical.Infrastructure.Configurations;

/// <summary>
/// Provides extension methods for configuring <see cref="BaseEntity"/> properties in Entity Framework Core.
/// </summary>
public static class BaseEntityConfigurationExtensions
{
    /// <summary>
    /// Configures the common properties of <see cref="BaseEntity"/> for the specified entity type.
    /// </summary>
    /// <typeparam name="T">The entity type that inherits from <see cref="BaseEntity"/>.</typeparam>
    /// <param name="builder">The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the entity type.</param>
    public static void ConfigureBaseEntity<T>(this EntityTypeBuilder<T> builder) where T : BaseEntity
    {
        builder.Property(prop => prop.Id)
            .IsRequired();

        builder.Property(prop => prop.InstanceId)
            .IsRequired();

        builder.Property(prop => prop.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(prop => prop.ModifiedBy)
            .HasMaxLength(255);
    }
}
