using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="Group"/> entity.
/// </summary>
public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    /// <summary>
    /// Configures the <see cref="Group"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(g => g.Id); 

        builder.Property(g => g.PartyId)
            .IsRequired(); 
        
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(255); 

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(512);

        builder.HasOne(g => g.Party)
            .WithOne() 
            .HasForeignKey<Group>(g => g.PartyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
