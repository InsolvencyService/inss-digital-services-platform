using INSS.Platform.Canonical.Infrastructure.Configurations;
using INSS.Platform.Canonical.Domain;
using Microsoft.EntityFrameworkCore;

namespace INSS.Platform.Canonical.Infrastructure;

/// <summary>
/// Represents the Entity Framework database context for the canonical domain.
/// Provides access to the canonical entities and configures their mappings.
/// </summary>
public class CanonicalDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CanonicalDbContext"/> class using the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
    public CanonicalDbContext(DbContextOptions<CanonicalDbContext> options) 
        : base(options) { }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="User"/> entities.
    /// </summary>
    public virtual DbSet<User> User { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Address"/> entities.
    /// </summary>
    public virtual DbSet<Address> Address { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="BankDetails"/> entities.
    /// </summary>
    public virtual DbSet<BankDetails> BankDetails { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{TEntity}"/> for <see cref="Income"/> entities.
    /// </summary>
    public virtual DbSet<Income> Income { get; set; }

    /// <summary>
    /// Configures the entity mappings for the canonical context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new BankDetailsConfiguration());
        modelBuilder.ApplyConfiguration(new IncomeConfiguration());
    }
}
