using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository for managing <see cref="AuthenticationProviderMetadata"/> entities.
    /// </summary>
    public class AuthenticationProviderMetadataRepository : RepositoryBase<AuthenticationProviderMetadata>, IAuthenticationProviderMetadataRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationProviderMetadataRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging repository operations.</param>
        /// <param name="context">The user management database context.</param>
        public AuthenticationProviderMetadataRepository(ILogger<AuthenticationProviderMetadataRepository> logger, UserManagementDbContext context) 
            : base(logger, context) { }
    }
}
