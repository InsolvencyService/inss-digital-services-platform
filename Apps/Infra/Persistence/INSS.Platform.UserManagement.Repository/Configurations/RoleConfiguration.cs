using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="Role"/> entity for Entity Framework Core.
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <summary>
        /// Configures the properties and relationships of the <see cref="Role"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.Description)
                .HasMaxLength(512);
        }
    }
}