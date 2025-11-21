using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.UserManagement.Repository.Configurations
{
    /// <summary>
    /// Configures the <see cref="PartyRelationship"/> entity for Entity Framework Core.
    /// </summary>
    public class PartyRelationshipConfiguration : IEntityTypeConfiguration<PartyRelationship>
    {
        /// <summary>
        /// Configures the <see cref="PartyRelationship"/> entity.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<PartyRelationship> builder)
        {
            builder.ConfigureAuditableEntity();

            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.FromPartyId)
                .IsRequired();

            builder.Property(pr => pr.ToPartyId)
                .IsRequired();

            builder.Property(pr => pr.RelationshipTypeId)
                .IsRequired();

            builder.Property(pr => pr.StartDate)
                .IsRequired();

            builder.Property(pr => pr.EndDate);

            builder.HasOne(pr => pr.FromParty)
                   .WithMany(p => p.RelationshipsFrom)
                   .HasForeignKey(pr => pr.FromPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pr => pr.ToParty)
                   .WithMany(p => p.RelationshipsTo)
                   .HasForeignKey(pr => pr.ToPartyId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pr => pr.RelationshipType)
                   .WithMany()
                   .HasForeignKey(pr => pr.RelationshipTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
