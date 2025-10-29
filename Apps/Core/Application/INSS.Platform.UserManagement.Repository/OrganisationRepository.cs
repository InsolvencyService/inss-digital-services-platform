using INSS.Platform.UserManagement.Entities;
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
        public async Task<OrganisationUser?> GetOrganisationUserAsync(Guid organisationId, Guid userId)
        {
            try
            {
                return await _dbContext.OrganisationUser
                    .SingleOrDefaultAsync(ou => ou.OrganisationId == organisationId && ou.UserId == userId)
                    .ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving organisation user with organisation ID {OrganisationId} and user ID {UserId}", userId, organisationId);
                return null;
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
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
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
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
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
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error deleting organisation with ID {OrganisationId}", organisation.Id);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> AddOrganisationUserAsync(OrganisationUser organisationUser)
        {
            try
            {
                _dbContext.OrganisationUser.Add(organisationUser);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error adding organisation user with user ID {UserId} organisation ID {OrganisationId}", organisationUser.UserId, organisationUser.OrganisationId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> OrganisationUserExistsAsync(Guid organisationId, Guid userId)
        {
            try
            {
                return await _dbContext.OrganisationUser.AnyAsync(ou => ou.OrganisationId == organisationId && ou.UserId == userId).ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error checking existence of organisation user with user ID {UserId} organisation ID {OrganisationId}", userId, organisationId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveOrganisationUserAsync(Guid organisationId, Guid userId)
        {
            try
            {
                OrganisationUser? organisationUser = await _dbContext.OrganisationUser
                    .FirstOrDefaultAsync(ou => ou.OrganisationId == organisationId && ou.UserId == userId)
                    .ConfigureAwait(false);

                if (organisationUser == null)
                {
                    _logger.LogWarning("Organisation user not found with user ID {UserId} organisation ID {OrganisationId}", userId, organisationId);
                    return false;
                }

                _dbContext.OrganisationUser.Remove(organisationUser);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error removing organisation user with user ID {UserId} organisation ID {OrganisationId}", userId, organisationId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveAllOrganisationUsersAsync(Guid organisationId)
        {
            try
            {
                IQueryable<OrganisationUser> organisationUsers = _dbContext.OrganisationUser.Where(ou => ou.OrganisationId == organisationId);
                _dbContext.OrganisationUser.RemoveRange(organisationUsers);

                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error removing all organisation users for organisation ID {OrganisationId}", organisationId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> AddOrganisationUserApplicationRoleAsync(OrganisationUserApplicationRole organisationUserApplicationRole)
        {
            try
            {
                _dbContext.OrganisationUserApplicationRole.Add(organisationUserApplicationRole);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error adding organisation user application role with organisation user ID {OrganisationUserId} and application role ID {ApplicationRoleId}", organisationUserApplicationRole.OrganisationUserId, organisationUserApplicationRole.ApplicationRoleId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> OrganisationUserApplicationRoleExistsAsync(Guid organisationUserId, Guid applicationRoleId)
        {
            try
            {
                return await _dbContext.OrganisationUserApplicationRole.AnyAsync(ouar => ouar.OrganisationUserId == organisationUserId && ouar.ApplicationRoleId == applicationRoleId).ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error checking existence of organisation user application role with organisation user ID {OrganisationUserId} and application role ID {ApplicationRoleId}", organisationUserId, applicationRoleId);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemoveOrganisationUserApplicationRoleAsync(Guid organisationUserId, Guid applicationRoleId)
        {
            try
            {
                OrganisationUserApplicationRole? organisationUserApplicationRole = await _dbContext.OrganisationUserApplicationRole
                    .FirstOrDefaultAsync(ouar => ouar.OrganisationUserId == organisationUserId && ouar.ApplicationRoleId == applicationRoleId)
                    .ConfigureAwait(false);

                if (organisationUserApplicationRole == null)
                {
                    _logger.LogWarning("Organisation user application role not found with organisation user ID {OrganisationUserId} and application role ID {ApplicationRoleId}", organisationUserId, applicationRoleId);
                    return false;
                }

                _dbContext.OrganisationUserApplicationRole.Remove(organisationUserApplicationRole);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error removing organisation user application role with organisation user ID {OrganisationUserId} and application role ID {ApplicationRoleId}", organisationUserId, applicationRoleId);
                return false;
            }
        }
    }
}
