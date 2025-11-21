using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="RoleMetadata"/> entities.
    /// </summary>
    public class RoleMetadataRepository : RepositoryBase<RoleMetadata>, IRoleMetadataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoleMetadataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging repository operations.</param>
        /// <param name="context">The database context for user management.</param>
        public RoleMetadataRepository(ILogger<RoleMetadataRepository> logger, UserManagementDbContext context) 
            : base(logger, context) 
        { }
    }
}
