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
    }
}
