using INSS.Platform.UserManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Provides methods for managing user data in the underlying database.
    /// </summary>
    /// <remarks>
    /// This repository is responsible for performing CRUD operations on user entities. It interacts
    /// with the database context to retrieve, add, update, and delete user records. Logging is used to capture errors
    /// during database operations.
    /// </remarks>
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly UserManagementDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.   
        /// </summary>
        /// <remarks>
        /// This constructor sets up the <see cref="UserRepository"/> with the necessary
        /// dependencies for logging and database access. Ensure that both <paramref name="logger"/> and <paramref name="dbContext"/> are not null when calling this constructor.
        /// </remarks>
        /// <param name="logger">The logger instance used to log messages related to user repository operations.</param>
        /// <param name="dbContext">The database context used to interact with the user management database.</param>
        public UserRepository(ILogger<UserRepository> logger, UserManagementDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            try
            {
                return await _dbContext.User
                    .Include(u => u.UserIdentity)
                    .SingleOrDefaultAsync(u => u.Id == userId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByIdentityProviderUserIdAsync(string identityProviderUserId, Guid identityProviderId)
        {
            try
            {
                return await _dbContext.User
                    .Include(u => u.UserIdentity)
                    .FirstOrDefaultAsync(
                        u => u.UserIdentity != null
                        && u.UserIdentity.IdentityProviderUserId == identityProviderUserId
                        && u.UserIdentity.IdentityProviderId == identityProviderId).ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving user with IdentityProviderUserId {IdentityProviderUserId} and IdentityProviderId {IdentityProviderId}", identityProviderUserId, identityProviderId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetUsersByOrganisationAsync(Guid organisationId)
        {
            try
            {
                return await (from ou in _dbContext.OrganisationUser
                              join user in _dbContext.User on ou.UserId equals user.Id
                              where ou.OrganisationId == organisationId
                              select user).ToListAsync().ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving users for organisation with ID {OrganisationId}", organisationId);
                return [];
            }
        }

        /// <inheritdoc />
        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                _dbContext.User.Add(user);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error adding user with email {Email}", user.Email);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserAsync(User user)
        {
            try
            {
                _dbContext.User.Update(user);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error updating user with ID {UserId}", user.Id);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteUserAsync(User user)
        {
            try
            {
                _dbContext.User.Remove(user);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception ex)
            when (ex is SqlException or DbUpdateException)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", user.Id);
                return false;
            }
        }
    }
}
