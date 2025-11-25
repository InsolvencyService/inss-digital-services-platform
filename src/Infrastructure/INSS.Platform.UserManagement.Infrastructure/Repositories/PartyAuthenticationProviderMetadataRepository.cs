using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

/// <summary>
/// Repository for managing <see cref="PartyAuthenticationProviderMetadata"/> entities.
/// </summary>
public class PartyAuthenticationProviderMetadataRepository : RepositoryBase<PartyAuthenticationProviderMetadata>, IPartyAuthenticationProviderMetadataRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PartyAuthenticationProviderMetadataRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public PartyAuthenticationProviderMetadataRepository(ILogger<PartyAuthenticationProviderMetadataRepository> logger, UserManagementDbContext context) 
        : base(logger, context) { }
}
