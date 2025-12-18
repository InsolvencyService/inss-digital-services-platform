using INSS.Platform.Canonical.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace INSS.Platform.Canonical.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for the <see cref="User"/> entity.
/// Configures properties, relationships, and constraints for the User table.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the <see cref="User"/> entity type.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ConfigureBaseEntity();

        builder.HasKey(user => user.Id);

        builder.Property(user => user.DateOfBirth)
            .IsRequired();

        builder.Property(user => user.TelephoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(user => user.EmailAddress)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasMany(user => user.Addresses)
            .WithOne(address => address.User)
            .HasForeignKey(address => address.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.BankDetails)
            .WithOne(bankDetails => bankDetails.User)
            .HasForeignKey(bankDetails => bankDetails.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.Incomes)
            .WithOne(income => income.User)
            .HasForeignKey(income => income.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
