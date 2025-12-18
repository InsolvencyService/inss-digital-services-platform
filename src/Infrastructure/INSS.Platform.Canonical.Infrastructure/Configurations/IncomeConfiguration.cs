using INSS.Platform.Canonical.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.Canonical.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="Income"/> entity.
/// </summary>
public class IncomeConfiguration : IEntityTypeConfiguration<Income>
{
    /// <summary>
    /// Configures the <see cref="Income"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Income> builder)
    {
        builder.ConfigureBaseEntity();

        builder.HasKey(income => income.Id); 

        builder.Property(income => income.UserId)
            .IsRequired(); 
        
        builder.Property(income => income.SourceOfIncome)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(income => income.GrossIncome)
            .IsRequired();
        
        builder.Property(income => income.PaymentFrequency)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(income => income.IncomeProvider)
            .IsRequired()
            .HasMaxLength(255);
    }
}
