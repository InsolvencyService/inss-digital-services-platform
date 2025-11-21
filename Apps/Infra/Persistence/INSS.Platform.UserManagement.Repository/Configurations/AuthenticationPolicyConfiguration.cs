using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the <see cref="Address"/> entity.
    /// </summary>
    public class AuthenticationPolicyConfiguration : IEntityTypeConfiguration<AuthenticationPolicy>
    {
        /// <summary>
        /// Configures the <see cref="AuthenticationPolicy"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<AuthenticationPolicy> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(ap => ap.Id);

            builder.Property(ap => ap.Name)
                .IsRequired()
                .HasMaxLength(255);
            
            builder.Property(ap => ap.Description)
                .IsRequired()
                .HasMaxLength(512);
            
            builder.Property(ap => ap.RequireMultiFactorAuthentication)
                .IsRequired();
            
            builder.Property(ap => ap.RequireIdentityVerification)
                .IsRequired();
        }
    }
}