using INSS.Platform.UserManagement.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Tests.Helpers;

/// <summary>
/// Test repository for <see cref="TestEntity"/> used in unit tests.
/// Inherits from <see cref="RepositoryBase{TestEntity}"/>.
/// </summary>
public class TestRepository : RepositoryBase<TestEntity>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for the repository base.</param>
    /// <param name="context">The user management database context.</param>
    public TestRepository(ILogger<RepositoryBase<TestEntity>> logger, UserManagementDbContext context)
        : base(logger, context) { }
}
