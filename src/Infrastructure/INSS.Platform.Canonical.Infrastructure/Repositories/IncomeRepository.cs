using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.Canonical.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Income"/> entities.
/// </summary>
public class IncomeRepository : RepositoryBase<Income>, IIncomeRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncomeRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public IncomeRepository(ILogger<IncomeRepository> logger, CanonicalDbContext context) 
        : base(logger, context) { }
}
