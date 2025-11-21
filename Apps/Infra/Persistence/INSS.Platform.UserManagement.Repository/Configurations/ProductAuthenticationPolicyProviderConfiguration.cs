using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="ProductAuthenticationPolicyProvider"/> entity for Entity Framework Core.
    /// </summary>
    public class ProductAuthenticationPolicyProviderConfiguration : IEntityTypeConfiguration<ProductAuthenticationPolicyProvider>
    {
        /// <summary>
        /// Configures the <see cref="ProductAuthenticationPolicyProvider"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ProductAuthenticationPolicyProvider> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(papp => papp.Id);

            builder.Property(papp => papp.ProductId)
                .IsRequired();

            builder.Property(papp => papp.AuthenticationPolicyProviderId)
                .IsRequired();

            builder.HasOne<Product>()
                   .WithMany()
                   .HasForeignKey(papp => papp.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<AuthenticationPolicyProvider>()
                   .WithMany()
                   .HasForeignKey(papp => papp.AuthenticationPolicyProviderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
