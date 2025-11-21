using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="Resource"/> entity for Entity Framework Core.
    /// </summary>
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        /// <summary>
        /// Configures the <see cref="Resource"/> entity properties and relationships.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(512);
        }
    }
}