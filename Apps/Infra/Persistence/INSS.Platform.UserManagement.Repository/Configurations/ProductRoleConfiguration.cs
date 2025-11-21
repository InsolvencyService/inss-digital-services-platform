using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="ProductRole"/> entity for Entity Framework Core.
    /// </summary>
    public class ProductRoleConfiguration : IEntityTypeConfiguration<ProductRole>
    {
        /// <summary>
        /// Configures the schema needed for the <see cref="ProductRole"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ProductRole> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.ProductId)
                .IsRequired();

            builder.Property(pr => pr.RoleId)
                .IsRequired();

            builder.HasOne<Product>()
                .WithMany()
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(pr => pr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}