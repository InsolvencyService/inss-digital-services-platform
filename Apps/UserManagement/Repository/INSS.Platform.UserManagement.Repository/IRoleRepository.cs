using INSS.Platform.UserManagement.Core.Entities;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Defines methods for managing and retrieving <see cref="Role"/> entities from a data store.
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Retrieves an Role by their unique identifier.
        /// </summary>
        /// <param name="RoleId">The unique identifier of the Role.</param>
        /// <returns>The <see cref="Role"/> if found; otherwise, <c>null</c>.</returns>
        Task<Role?> GetRoleByIdAsync(Guid roleId);

        /// <summary>
        /// Retrieves a collection of <see cref="Role"/> entities associated with the specified organisation, user, and application.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{Role}"/>
        /// of roles matching the specified organisation, user, and application.
        /// </returns>
        Task<IEnumerable<Role>> GetRolesByOrganisationUserApplicationAsync(Guid organisationId, Guid userId, Guid applicationId);

        /// <summary>
        /// Adds a new Role to the data store.
        /// </summary>
        /// <param name="Role">The <see cref="Role"/> to add.</param>
        Task<bool> AddRoleAsync(Role role);

        /// <summary>
        /// Updates an existing Role in the data store.
        /// </summary>
        /// <param name="Role">The <see cref="Role"/> to update.</param>
        Task<bool> UpdateRoleAsync(Role role);

        /// <summary>
        /// Deletes a Role from the data store.
        /// </summary>
        /// <param name="Role">The <see cref="Role"/> to delete.</param>
        Task<bool> DeleteRoleAsync(Role role);
    }
}
