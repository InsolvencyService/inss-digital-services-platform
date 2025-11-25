using INSS.Platform.UserManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Infrastructure.Configurations;

/// <summary>
/// Configures the <see cref="AuthenticationPolicyProvider"/> entity for Entity Framework Core.
/// </summary>
public class AuthenticationPolicyProviderConfiguration : IEntityTypeConfiguration<AuthenticationPolicyProvider>
{
    /// <summary>
    /// Configures the entity type builder for <see cref="AuthenticationPolicyProvider"/>.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<AuthenticationPolicyProvider> builder)
    {
        builder.ConfigureAuditableEntity();

        builder.HasKey(app => app.Id);

        builder.Property(app => app.AuthenticationPolicyId)
            .IsRequired();
        
        builder.Property(app => app.AuthenticationProviderId)
            .IsRequired();

        builder.HasOne<AuthenticationPolicy>()
            .WithMany()
            .HasForeignKey(app => app.AuthenticationPolicyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AuthenticationProvider>()
            .WithMany()
            .HasForeignKey(app => app.AuthenticationProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}