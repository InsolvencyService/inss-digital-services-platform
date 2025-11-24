using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="Individual"/> entity.
/// </summary>
public class IndividualConfiguration : IEntityTypeConfiguration<Individual>
{
    /// <summary>
    /// Configures the schema needed for the <see cref="Individual"/> entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Individual> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(i => i.Id);

        builder.Property(i => i.PartyId)
            .IsRequired();

        builder.Property(i => i.FirstName)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(i => i.LastName)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(i => i.NationalInsuranceNumber)
            .HasMaxLength(9);
        
        builder.Property(i => i.IsIdentityVerified)
            .IsRequired();
        
        builder.Property(i => i.VerificationSource)
            .HasMaxLength(255);

        builder.HasOne(g => g.Party)
            .WithOne()
            .HasForeignKey<Individual>(g => g.PartyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
