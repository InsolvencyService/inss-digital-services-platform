using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="PartyType"/> entities.
    /// </summary>
    public class PartyTypeRepository : RepositoryBase<PartyType>, IPartyTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartyTypeRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="context">The database context for user management.</param>
        public PartyTypeRepository(ILogger<PartyTypeRepository> logger, UserManagementDbContext context) 
            : base(logger, context) { }
    }
}
