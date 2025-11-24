using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

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
        builder.ConfigureAuditableEntity();

        builder.HasKey(a => a.Id); 

        builder.Property(a => a.PartyId)
            .IsRequired(); 
        
        builder.Property(a => a.AddressTypeId)
            .IsRequired(); 
        
        builder.Property(a => a.AddressLine1)
            .IsRequired().HasMaxLength(255); 

        builder.Property(a => a.AddressLine2)
            .IsRequired()
            .HasMaxLength(255); 
        
        builder.Property(a => a.AddressLine3)
            .IsRequired()
            .HasMaxLength(255); 
        
        builder.Property(a => a.Postcode)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasOne(a => a.Party)
            .WithMany(p => p.Addresses)
            .HasForeignKey(a => a.PartyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
