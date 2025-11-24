using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="AuthenticationProvider"/> entity.
/// </summary>
public class AuthenticationProviderConfiguration : IEntityTypeConfiguration<AuthenticationProvider>
{
    /// <summary>
    /// Configures the <see cref="AuthenticationProvider"/> entity properties and relationships.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<AuthenticationProvider> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(ap => ap.Id);

        builder.Property(ap => ap.Name)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(ap => ap.Description)
            .IsRequired()
            .HasMaxLength(512);
    }
}