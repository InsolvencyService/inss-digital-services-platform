using INSS.Platform.UserManagement.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Provides methods for managing role data in the underlying database.
    /// </summary>
    /// <remarks>
    /// This repository is responsible for performing CRUD operations on role entities. It interacts
    /// with the database context to retrieve, add, update, and delete role records. Logging is used to capture errors
    /// during database operations.
    /// </remarks>
    public class RoleRepository : IRoleRepository
    {
        private readonly ILogger<RoleRepository> _logger;
        private readonly UserManagementDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository"/> class.   
        /// </summary>
        /// <remarks>
        /// This constructor sets up the <see cref="RoleRepository"/> with the necessary
        /// dependencies for logging and database access. Ensure that both <paramref name="logger"/> and <paramref name="dbContext"/> are not null when calling this constructor.
        /// </remarks>
        /// <param name="logger">The logger instance used to log messages related to role repository operations.</param>
        /// <param name="dbContext">The database context used to interact with the role management database.</param>
        public RoleRepository(ILogger<RoleRepository> logger, UserManagementDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<Role?> GetRoleByIdAsync(Guid roleId)
        {
            try
            {
                return await _dbContext.Role.FindAsync(roleId).ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving role with ID {RoleId}", roleId);
                return null;
            }
        }

        public async Task<IEnumerable<Role>> GetRolesByOrganisationUserApplicationAsync(Guid organisationId, Guid userId, Guid applicationId)
        {
            try
            {
                OrganisationUser? orgUser = await _dbContext.OrganisationUser
                    .FirstOrDefaultAsync(ou => ou.OrganisationId == organisationId && ou.UserId == userId)
                    .ConfigureAwait(false);

                if (orgUser == null)
                {
                    return [];
                }

                return await (from role in _dbContext.Role
                              join approle in _dbContext.ApplicationRole on role.Id equals approle.RoleId
                              join ouar in _dbContext.OrganisationUserApplicationRole on approle.Id equals ouar.ApplicationRoleId
                              where approle.ApplicationId == applicationId && ouar.OrganisationUserId == orgUser.Id
                              select role).ToListAsync().ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving roles for organisation with ID {OrganisationId} and user with ID {UserId} and application with ID {ApplicationId}", organisationId, userId, applicationId);
                return [];
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Role>> GetRolesByApplicationAsync(Guid applicationId)
        {
            try
            {
                return await (from role in _dbContext.Role
                              join approle in _dbContext.ApplicationRole on role.Id equals approle.RoleId
                              where approle.ApplicationId == applicationId
                              select role).ToListAsync().ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving roles for application with ID {ApplicationId}", applicationId);
                return [];
            }
        }

        /// <inheritdoc />
        public async Task<bool> AddRoleAsync(Role role)
        {
            try
            {
                _dbContext.Role.Add(role);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error adding role with name {Name}", role.Name);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateRoleAsync(Role role)
        {
            try
            {
                _dbContext.Role.Update(role);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error updating role with ID {RoleId}", role.Id);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteRoleAsync(Role role)
        {
            try
            {
                _dbContext.Role.Remove(role);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error deleting role with ID {RoleId}", role.Id);
                return false;
            }
        }
    }
}
