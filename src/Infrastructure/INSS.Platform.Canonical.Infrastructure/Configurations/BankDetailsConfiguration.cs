using INSS.Platform.Canonical.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.Canonical.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="BankDetails"/> entity.
/// </summary>
public class BankDetailsConfiguration : IEntityTypeConfiguration<BankDetails>
{
    /// <summary>
    /// Configures the <see cref="BankDetails"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<BankDetails> builder)
    {
        builder.ConfigureBaseEntity();

        builder.HasKey(bankDetails => bankDetails.Id); 

        builder.Property(bankDetails => bankDetails.UserId)
            .IsRequired(); 
        
        builder.Property(bankDetails => bankDetails.AccountName)
            .IsRequired()
            .HasMaxLength(255); 

        builder.Property(bankDetails => bankDetails.SortCode)
            .IsRequired()
            .HasMaxLength(6); 
        
        builder.Property(bankDetails => bankDetails.AccountNumber)
            .IsRequired()
            .HasMaxLength(8);

        builder.Property(bankDetails => bankDetails.BuildingSocietyRollNumber)
            .HasMaxLength(20)
            .IsRequired(false);
    }
}
