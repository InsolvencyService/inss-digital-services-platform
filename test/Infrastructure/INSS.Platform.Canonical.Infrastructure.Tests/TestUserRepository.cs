using INSS.Platform.Canonical.Domain;
using INSS.Platform.Canonical.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.Canonical.Infrastructure.Tests;


public class TestUserRepository : RepositoryBase<User>
{
    public TestUserRepository(ILogger<RepositoryBase<User>> logger, CanonicalDbContext context) : base(logger, context) { }
}
