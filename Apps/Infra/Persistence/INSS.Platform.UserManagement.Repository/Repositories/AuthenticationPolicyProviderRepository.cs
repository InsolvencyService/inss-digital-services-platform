using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="AuthenticationPolicyProvider"/> entities.
    /// </summary>
    public class AuthenticationPolicyProviderRepository : RepositoryBase<AuthenticationPolicyProvider>, IAuthenticationPolicyProviderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPolicyProviderRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging repository operations.</param>
        /// <param name="context">The database context for user management.</param>
        public AuthenticationPolicyProviderRepository(ILogger<AuthenticationPolicyProviderRepository> logger, UserManagementDbContext context) 
            : base(logger, context) { }
    }
}
