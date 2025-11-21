using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="Organisation"/> entity for Entity Framework Core.
    /// </summary>
    public class OrganisationConfiguration : IEntityTypeConfiguration<Organisation>
    {
        /// <summary>
        /// Configures the schema needed for the <see cref="Organisation"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<Organisation> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(o => o.Id); 

            builder.Property(o => o.PartyId)
                .IsRequired(); 
            
            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(255); 

            builder.Property(o => o.Description)
                .IsRequired()
                .HasMaxLength(512);

            builder.HasOne(g => g.Party)
                .WithOne()
                .HasForeignKey<Organisation>(g => g.PartyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
