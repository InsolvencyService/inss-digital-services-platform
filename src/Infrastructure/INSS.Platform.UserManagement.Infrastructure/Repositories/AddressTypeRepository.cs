using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="AddressType"/> entities.
/// </summary>
public class AddressTypeRepository : RepositoryBase<AddressType>, IAddressTypeRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AddressTypeRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public AddressTypeRepository(ILogger<AddressTypeRepository> logger, UserManagementDbContext context) 
        : base(logger, context) { }
}
