using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Permission"/> entities.
/// </summary>
public class PermissionRepository : RepositoryBase<Permission>, IPermissionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public PermissionRepository(ILogger<PermissionRepository> logger, UserManagementDbContext context) 
        : base(logger, context) { }
}
