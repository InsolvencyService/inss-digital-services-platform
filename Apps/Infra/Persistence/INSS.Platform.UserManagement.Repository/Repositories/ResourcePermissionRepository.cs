using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="ResourcePermission"/> entities.
    /// </summary>
    public class ResourcePermissionRepository : RepositoryBase<ResourcePermission>, IResourcePermissionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePermissionRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging repository operations.</param>
        /// <param name="context">The database context for user management.</param>
        public ResourcePermissionRepository(ILogger<ResourcePermissionRepository> logger, UserManagementDbContext context) 
            : base(logger, context) { }
    }
}
