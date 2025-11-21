using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="ProductRoleResourcePermission"/> entity for Entity Framework Core.
    /// </summary>
    public class ProductRoleResourcePermissionConfiguration : IEntityTypeConfiguration<ProductRoleResourcePermission>
    {
        /// <summary>
        /// Configures the <see cref="ProductRoleResourcePermission"/> entity.
        /// Sets up keys, required properties, and relationships to <see cref="ProductRole"/> and <see cref="ResourcePermission"/>.
        /// </summary>
        /// <param name="builder">The builder used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ProductRoleResourcePermission> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(prrp => prrp.Id);

            builder.Property(prrp => prrp.ProductRoleId)
                .IsRequired();

            builder.Property(prrp => prrp.ResourcePermissionId)
                .IsRequired();

            builder.HasOne<ProductRole>()
                   .WithMany()
                   .HasForeignKey(prrp => prrp.ProductRoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ResourcePermission>()
                   .WithMany()
                   .HasForeignKey(prrp => prrp.ResourcePermissionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}