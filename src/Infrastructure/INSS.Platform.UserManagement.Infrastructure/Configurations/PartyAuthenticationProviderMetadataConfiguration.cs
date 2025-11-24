using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Configures the <see cref="PartyAuthenticationProviderMetadata"/> entity for Entity Framework Core.
/// </summary>
public class PartyAuthenticationProviderMetadataConfiguration : IEntityTypeConfiguration<PartyAuthenticationProviderMetadata>
{
    /// <summary>
    /// Configures the <see cref="PartyAuthenticationProviderMetadata"/> entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<PartyAuthenticationProviderMetadata> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(papm => papm.Id);

        builder.Property(papm => papm.PartyId)
            .IsRequired();

        builder.Property(papm => papm.AuthenticationPolicyProviderId)
            .IsRequired();

        builder.Property(papm => papm.AuthenticationProviderUserId)
            .IsRequired();

        builder.Property(papm => papm.AuthenticationProviderSessionData);

        builder.HasOne<Party>()
            .WithMany()
            .HasForeignKey(papm => papm.PartyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AuthenticationPolicyProvider>()
            .WithMany()
            .HasForeignKey(papm => papm.AuthenticationPolicyProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}