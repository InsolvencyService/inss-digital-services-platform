using INSS.Platform.UserManagement.Core.Entities;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Defines methods for managing and retrieving <see cref="Organisation"/> entities from a data store.
    /// </summary>
    public interface IOrganisationRepository
    {
        /// <summary>
        /// Retrieves an Organisation by their unique identifier.
        /// </summary>
        /// <param name="OrganisationId">The unique identifier of the Organisation.</param>
        /// <returns>The <see cref="Organisation"/> if found; otherwise, <c>null</c>.</returns>
        Task<Organisation?> GetOrganisationByIdAsync(Guid organisationId);

        /// <summary>
        /// Retrieves a collection of <see cref="Organisation"/> entities associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose organisations are to be retrieved.</param>
        /// <returns>An <see cref="IEnumerable{Organisation}"/> containing all organisations linked to the specified user.</returns>
        Task<IEnumerable<Organisation>> GetOrganisationsByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all <see cref="Organisation"/> entities from the data store.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation, 
        /// containing an <see cref="IEnumerable{Organisation}"/> of all organisations.
        /// </returns>
        Task<IEnumerable<Organisation>> GetOrganisationsAsync();

        /// <summary>
        /// Adds a new Organisation to the data store.
        /// </summary>
        /// <param name="Organisation">The <see cref="Organisation"/> to add.</param>
        Task<bool> AddOrganisationAsync(Organisation organisation);

        /// <summary>
        /// Updates an existing Organisation in the data store.
        /// </summary>
        /// <param name="Organisation">The <see cref="Organisation"/> to update.</param>
        Task<bool> UpdateOrganisationAsync(Organisation organisation);

        /// <summary>
        /// Deletes a Organisation from the data store.
        /// </summary>
        /// <param name="Organisation">The <see cref="Organisation"/> to delete.</param>
        Task<bool> DeleteOrganisationAsync(Organisation organisation);
    }
}
