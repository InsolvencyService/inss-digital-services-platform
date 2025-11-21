using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="PartyProductRole"/> entity for Entity Framework Core.
    /// </summary>
    public class PartyProductRoleConfiguration : IEntityTypeConfiguration<PartyProductRole>
    {
        /// <summary>
        /// Configures the <see cref="PartyProductRole"/> entity.
        /// Sets up keys, required properties, and relationships to <see cref="Party"/> and <see cref="ProductRole"/>.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PartyProductRole> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(ppr => ppr.Id);

            builder.Property(ppr => ppr.PartyId)
                .IsRequired();

            builder.Property(ppr => ppr.ProductRoleId)
                .IsRequired();

            builder.HasOne<Party>()
                .WithMany()
                .HasForeignKey(ppr => ppr.PartyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ProductRole>()
                .WithMany()
                .HasForeignKey(ppr => ppr.ProductRoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}