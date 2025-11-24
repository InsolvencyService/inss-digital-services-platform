using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="AuthenticationPolicy"/> entities.
/// </summary>
public class AuthenticationPolicyRepository : RepositoryBase<AuthenticationPolicy>, IAuthenticationPolicyRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationPolicyRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public AuthenticationPolicyRepository(ILogger<AuthenticationPolicyRepository> logger, UserManagementDbContext context) 
        : base(logger, context) { }
}
