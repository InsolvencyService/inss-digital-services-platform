using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="Resource"/> entities.
    /// </summary>
    public class ResourceRepository : RepositoryBase<Resource>, IResourceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging repository operations.</param>
        /// <param name="context">The database context for user management.</param>
        public ResourceRepository(ILogger<ResourceRepository> logger, UserManagementDbContext context) 
            : base(logger, context) { }
    }
}
