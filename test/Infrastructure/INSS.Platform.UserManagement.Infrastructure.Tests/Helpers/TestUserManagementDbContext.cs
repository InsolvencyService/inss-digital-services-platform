using Microsoft.EntityFrameworkCore;

namespace INSS.Platform.UserManagement.Infrastructure.Tests.Helpers;

/// <summary>
/// A test-specific version of <see cref="UserManagementDbContext"/> to support the BaseRepository tests.
/// </summary>
public class TestUserManagementDbContext : UserManagementDbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestUserManagementDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="UserManagementDbContext"/>.</param>
    public TestUserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
        : base(options) { }

    /// <summary>
    /// Configures the model for the context being created.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TestEntity>();
    }
}
