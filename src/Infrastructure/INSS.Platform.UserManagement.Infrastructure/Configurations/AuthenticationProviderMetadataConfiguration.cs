using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Configures the <see cref="AuthenticationProviderMetadata"/> entity for Entity Framework Core.
/// </summary>
public class AuthenticationProviderMetadataConfiguration : IEntityTypeConfiguration<AuthenticationProviderMetadata>
{
    /// <summary>
    /// Configures the schema needed for the <see cref="AuthenticationProviderMetadata"/> entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<AuthenticationProviderMetadata> builder)
    {
        builder.ConfigureAuditableEntity();
        
        builder.HasKey(apm => apm.Id);
        
        builder.Property(apm => apm.AuthenticationProviderId)
            .IsRequired();

        builder.Property(apm => apm.ClientId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(apm => apm.Secret)
            .HasMaxLength(100);

        builder.Property(apm => apm.AuthorizeEndpoint)
            .HasMaxLength(2083);

        builder.Property(apm => apm.TokenEndpoint)
            .HasMaxLength(2083);

        builder.HasOne<AuthenticationProvider>()
            .WithMany()
            .HasForeignKey(apm => apm.AuthenticationProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}