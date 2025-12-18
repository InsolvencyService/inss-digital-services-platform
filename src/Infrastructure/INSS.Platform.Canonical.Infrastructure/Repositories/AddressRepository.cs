using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.Canonical.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Address"/> entities.
/// </summary>
public class AddressRepository : RepositoryBase<Address>, IAddressRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public AddressRepository(ILogger<AddressRepository> logger, CanonicalDbContext context) 
        : base(logger, context) { }
}
