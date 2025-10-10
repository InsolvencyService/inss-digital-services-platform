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
        /// Retrieves the <see cref="OrganisationUser"/> entity representing the association between a specified organisation and user.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A <see cref="Task{OrganisationUser}"/> representing the asynchronous operation,
        /// containing the <see cref="OrganisationUser"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<OrganisationUser?> GetOrganisationUserAsync(Guid organisationId, Guid userId);

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

        /// <summary>
        /// Adds a new association between an organisation and a user to the data store.
        /// </summary>
        /// <param name="organisationUser">
        /// The <see cref="OrganisationUser"/> entity representing the association to add.
        /// </param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation, 
        /// containing <c>true</c> if the association was added successfully; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddOrganisationUserAsync(OrganisationUser organisationUser);

        /// <summary>
        /// Determines whether an association exists between a specified organisation and user.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation,
        /// containing <c>true</c> if the association exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> OrganisationUserExistsAsync(Guid organisationId, Guid userId);

        /// <summary>
        /// Determines whether an association exists between a specified organisation user and application role.
        /// </summary>
        /// <param name="organisationUserId">The unique identifier of the organisation user.</param>
        /// <param name="applicationRoleId">The unique identifier of the application role.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation,
        /// containing <c>true</c> if the association exists; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> OrganisationUserApplicationRoleExistsAsync(Guid organisationUserId, Guid applicationRoleId);

        /// <summary>
        /// Adds a new association between an organisation user and an application role to the data store.
        /// </summary>
        /// <param name="organisationUserApplicationRole">
        /// The <see cref="OrganisationUserApplicationRole"/> entity representing the association to add.
        /// </param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation,
        /// containing <c>true</c> if the association was added successfully; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AddOrganisationUserApplicationRoleAsync(OrganisationUserApplicationRole organisationUserApplicationRole);

        /// <summary>
        /// Removes an association between an organisation user and an application role from the data store.
        /// </summary>
        /// <param name="organisationUserId">The unique identifier of the organisation user.</param>
        /// <param name="applicationRoleId">The unique identifier of the application role.</param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation,
        /// containing <c>true</c> if the association was removed successfully; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveOrganisationUserApplicationRoleAsync(Guid organisationUserId, Guid applicationRoleId);

        /// <summary>
        /// Removes an association between an organisation and a user from the data store.
        /// </summary>
        /// <param name="organisationUser">
        /// The <see cref="OrganisationUser"/> entity representing the association to remove.
        /// </param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation,
        /// containing <c>true</c> if the association was removed successfully; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveOrganisationUserAsync(Guid organisationId, Guid userId);

        /// <summary>
        /// Removes all user associations for a specified organisation from the data store.
        /// </summary>
        /// <param name="organisationId">
        /// The unique identifier of the organisation whose user associations are to be removed.
        /// </param>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> representing the asynchronous operation,
        /// containing <c>true</c> if all associations were removed successfully; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> RemoveAllOrganisationUsersAsync(Guid organisationId);
    }
}
