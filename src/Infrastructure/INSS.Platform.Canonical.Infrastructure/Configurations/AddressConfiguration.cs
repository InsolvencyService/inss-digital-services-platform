using INSS.Platform.Canonical.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.Canonical.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="Address"/> entity.
/// </summary>
public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    /// <summary>
    /// Configures the <see cref="Address"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ConfigureBaseEntity();

        builder.HasKey(address => address.Id); 

        builder.Property(address => address.UserId)
            .IsRequired(); 
        
        builder.Property(address => address.AddressLine1)
            .IsRequired().HasMaxLength(255); 

        builder.Property(address => address.AddressLine2)
            .IsRequired(false)
            .HasMaxLength(255); 
        
        builder.Property(address => address.TownCity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(address => address.County)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(address => address.Postcode)
            .IsRequired()
            .HasMaxLength(20);
    }
}
