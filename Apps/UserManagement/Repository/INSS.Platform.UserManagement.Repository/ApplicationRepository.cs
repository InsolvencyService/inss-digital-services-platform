using INSS.Platform.UserManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Provides methods for managing application data in the underlying database.
    /// </summary>
    /// <remarks>
    /// This repository is responsible for performing CRUD operations on application entities. It interacts
    /// with the database context to retrieve, add, update, and delete application records. Logging is used to capture errors
    /// during database operations.
    /// </remarks>
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ILogger<ApplicationRepository> _logger;
        private readonly UserManagementDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationRepository"/> class.   
        /// </summary>
        /// <remarks>
        /// This constructor sets up the <see cref="ApplicationRepository"/> with the necessary
        /// dependencies for logging and database access. Ensure that both <paramref name="logger"/> and <paramref name="dbContext"/> are not null when calling this constructor.
        /// </remarks>
        /// <param name="logger">The logger instance used to log messages related to application repository operations.</param>
        /// <param name="dbContext">The database context used to interact with the application management database.</param>
        public ApplicationRepository(ILogger<ApplicationRepository> logger, UserManagementDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<Application?> GetApplicationByIdAsync(Guid applicationId)
        {
            try
            {
                return await _dbContext.Application.FindAsync(applicationId).ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving application with ID {ApplicationId}", applicationId);
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Application>> GetApplicationsAsync()
        {
            try
            {
                return await _dbContext.Application.ToListAsync().ConfigureAwait(false);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving all applications");
                return [];
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Application>> GetApplicationsByOrganisationUserAsync(Guid organisationId, Guid userId)
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

                return from app in _dbContext.Application
                       join approle in _dbContext.ApplicationRole on app.Id equals approle.ApplicationId
                       join ouar in _dbContext.OrganisationUserApplicationRole on approle.Id equals ouar.ApplicationRoleId
                       where ouar.OrganisationUserId == orgUser.Id
                       select app;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error retrieving applications for organisation with ID {OrganisationId} and user with ID {UserId}", organisationId, userId);
                return [];
            }
        }

        /// <inheritdoc />
        public async Task<bool> AddApplicationAsync(Application application)
        {
            try
            {
                _dbContext.Application.Add(application);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error adding application with name {Name}", application.Name);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateApplicationAsync(Application application)
        {
            try
            {
                _dbContext.Application.Update(application);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error updating application with ID {ApplicationId}", application.Id);
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteApplicationAsync(Application application)
        {
            try
            {
                _dbContext.Application.Remove(application);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return true;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error deleting application with ID {ApplicationId}", application.Id);
                return false;
            }
        }
    }
}
