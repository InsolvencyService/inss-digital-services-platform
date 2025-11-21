using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="Party"/> entity for Entity Framework Core.
    /// </summary>
    public class PartyConfiguration : IEntityTypeConfiguration<Party>
    {
        /// <summary>
        /// Configures the entity of type <see cref="Party"/>.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Party> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PartyTypeId)
                .IsRequired();
            
            builder.Property(p => p.SourceOfIntroduction)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(p => p.PartyType)
                .WithMany()
                .HasForeignKey(p => p.PartyTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
