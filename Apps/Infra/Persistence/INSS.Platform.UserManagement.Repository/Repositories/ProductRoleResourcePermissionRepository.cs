using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="ProductRoleResourcePermission"/> entities.
    /// </summary>
    public class ProductRoleResourcePermissionRepository : RepositoryBase<ProductRoleResourcePermission>, IProductRoleResourcePermissionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductRoleResourcePermissionRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging repository operations.</param>
        /// <param name="context">The database context for user management.</param>
        public ProductRoleResourcePermissionRepository(ILogger<ProductRoleResourcePermissionRepository> logger, UserManagementDbContext context) 
            : base(logger, context) { }
    }
}
