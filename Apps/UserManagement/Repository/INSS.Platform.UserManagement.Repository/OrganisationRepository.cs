using INSS.Platform.UserManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Provides methods for managing organisation data in the underlying database.
    /// </summary>
    /// <remarks>
    /// This repository is responsible for performing CRUD operations on organisation entities. It interacts
    /// with the database context to retrieve, add, update, and delete organisation records. Logging is used to capture errors
    /// during database operations.
    /// </remarks>
    public class OrganisationRepository : IOrganisationRepository
    {
        private readonly ILogger<OrganisationRepository> _logger;
        private readonly UserManagementDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationRepository"/> class.   
        /// </summary>
        /// <remarks>
        /// This constructor sets up the <see cref="OrganisationRepository"/> with the necessary
        /// dependencies for logging and database access. Ensure that both <paramref name="logger"/> and <paramref name="dbContext"/> are not null when calling this constructor.
        /// </remarks>
        /// <param name="logger">The logger instance used to log messages related to organisation repository operations.</param>
        /// <param name="dbContext">The database context used to interact with the organisation management database.</param>
        public OrganisationRepository(ILogger<OrganisationRepository> logger, UserManagementDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<Organisation?> GetOrganisationByIdAsync(Guid organisationId)
        {
            try
            {
                return await _dbContext.Organisation.FindAsync(organisationId).ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving organisation with ID {OrganisationId}", organisationId);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Organisation>> GetOrganisationsByUserIdAsync(Guid userId)
        {
            try
            {
                return await (from ou in _dbContext.OrganisationUser
                              join org in _dbContext.Organisation on ou.OrganisationId equals org.Id
                              where ou.UserId == userId
                              select org).ToListAsync().ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving organisations for user with ID {UserId}", userId);
                return [];
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Organisation>> GetOrganisationsAsync()
        {
            try
            {
                return await _dbContext.Organisation.ToListAsync().ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving all organisations");
                return [];
            }
        }


        /// <inheritdoc />
        public async Task<bool> AddOrganisationAsync(Organisation organisation)
        {
            try
            {
                _dbContext.Organisation.Add(organisation);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error adding organisation with name {Name}", organisation.Name);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateOrganisationAsync(Organisation organisation)
        {
            try
            {
                _dbContext.Organisation.Update(organisation);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error updating organisation with ID {OrganisationId}", organisation.Id);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteOrganisationAsync(Organisation organisation)
        {
            try
            {
                _dbContext.Organisation.Remove(organisation);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error deleting organisation with ID {OrganisationId}", organisation.Id);
                return false;
            }
        }
    }
}
