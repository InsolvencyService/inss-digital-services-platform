using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="ResourcePermission"/> entity for Entity Framework Core.
    /// </summary>
    public class ResourcePermissionConfiguration : IEntityTypeConfiguration<ResourcePermission>
    {
        /// <summary>
        /// Configures the schema needed for the <see cref="ResourcePermission"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ResourcePermission> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(rp => rp.Id);

            builder.Property(rp => rp.ResourceId)
                .IsRequired();

            builder.Property(rp => rp.PermissionId)
                .IsRequired();

            builder.HasOne<Resource>()
                   .WithMany()
                   .HasForeignKey(rp => rp.ResourceId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Permission>()
                   .WithMany()
                   .HasForeignKey(rp => rp.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}