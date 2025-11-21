using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="Permission"/> entity for Entity Framework Core.
    /// </summary>
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        /// <summary>
        /// Configures the schema needed for the <see cref="Permission"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(512);
        }
    }
}