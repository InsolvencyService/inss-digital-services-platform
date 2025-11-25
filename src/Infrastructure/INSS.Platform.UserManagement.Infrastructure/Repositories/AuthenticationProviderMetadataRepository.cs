using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

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
