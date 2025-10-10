using INSS.Platform.UserManagement.Core.Entities;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Defines methods for managing and retrieving <see cref="Application"/> entities from a data store.
    /// </summary>
    public interface IApplicationRepository
    {
        /// <summary>
        /// Retrieves an Application by their unique identifier.
        /// </summary>
        /// <param name="ApplicationId">The unique identifier of the Application.</param>
        /// <returns>The <see cref="Application"/> if found; otherwise, <c>null</c>.</returns>
        Task<Application?> GetApplicationByIdAsync(Guid applicationId);

        /// <summary>
        /// Retrieves all <see cref="Application"/> entities from the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Application}"/> containing all applications.</returns>
        Task<IEnumerable<Application>> GetApplicationsAsync();

        /// <summary>
        /// Retrieves all <see cref="Application"/> entities associated with a specific organisation and user.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// An <see cref="IEnumerable{Application}"/> containing applications linked to the specified organisation and user.
        /// </returns>
        Task<IEnumerable<Application>> GetApplicationsByOrganisationUserAsync(Guid organisationId, Guid userId);

        /// <summary>
        /// Retrieves the <see cref="ApplicationRole"/> associated with the specified application and role identifiers.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <param name="roleId">The unique identifier of the role.</param>
        /// <returns>
        /// A <see cref="Task{ApplicationRole}"/> representing the asynchronous operation.
        /// The task result contains the <see cref="ApplicationRole"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<ApplicationRole?> GetApplicationRoleAsync(Guid applicationId, Guid roleId);

        /// <summary>
        /// Adds a new Application to the data store.
        /// </summary>
        /// <param name="Application">The <see cref="Application"/> to add.</param>
        Task<bool> AddApplicationAsync(Application application);

        /// <summary>
        /// Updates an existing Application in the data store.
        /// </summary>
        /// <param name="Application">The <see cref="Application"/> to update.</param>
        Task<bool> UpdateApplicationAsync(Application application);

        /// <summary>
        /// Deletes a Application from the data store.
        /// </summary>
        /// <param name="Application">The <see cref="Application"/> to delete.</param>
        Task<bool> DeleteApplicationAsync(Application application);

        /// <summary>
        /// Adds a new <see cref="ApplicationRole"/> association to the data store.
        /// </summary>
        /// <param name="applicationRole">
        /// The <see cref="ApplicationRole"/> entity representing the association between an application and a role to add.
        /// </param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation.
        /// The task result contains <c>true</c> if the association was successfully added; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddApplicationRoleAsync(ApplicationRole applicationRole);

        /// <summary>
        /// Determines whether a specific role exists for the given application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <param name="roleId">The unique identifier of the role.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation.
        /// The task result contains <c>true</c> if the role exists for the application; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> ApplicationRoleExistsAsync(Guid applicationId, Guid roleId);

        /// <summary>
        /// Removes a specific role from the given application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <param name="roleId">The unique identifier of the role to remove.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation.
        /// The task result contains <c>true</c> if the role was successfully removed; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveApplicationRoleAsync(Guid applicationId, Guid roleId);

        /// <summary>
        /// Removes all roles associated with the given application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation.
        /// The task result contains <c>true</c> if all roles were successfully removed; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveAllApplicationRolesAsync(Guid applicationId);
    }
}
