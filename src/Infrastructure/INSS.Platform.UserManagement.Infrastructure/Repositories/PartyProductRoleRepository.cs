using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="PartyProductRole"/> entities.
/// </summary>
public class PartyProductRoleRepository : RepositoryBase<PartyProductRole>, IPartyProductRoleRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartyProductRoleRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public PartyProductRoleRepository(ILogger<PartyProductRoleRepository> logger, UserManagementDbContext context) 
        : base(logger, context) { }
}
