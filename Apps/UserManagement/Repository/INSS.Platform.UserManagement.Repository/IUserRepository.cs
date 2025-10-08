using INSS.Platform.UserManagement.Core.Entities;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Defines methods for managing and retrieving <see cref="User"/> entities from a data store.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        Task<User?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Retrieves a user by their identity provider user ID and identity provider ID.
        /// </summary>
        /// <param name="identityProviderUserId">The unique user identifier provided by the identity provider.</param>
        /// <param name="identityProviderId">The unique identifier of the identity provider.</param>
        /// <returns>The <see cref="User"/> if found; otherwise, <c>null</c>.</returns>
        Task<User?> GetUserByIdentityProviderUserIdAsync(string identityProviderUserId, Guid identityProviderId);

        /// <summary>
        /// Retrieves all users associated with a specific organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <returns>An <see cref="IEnumerable{User}"/> containing users belonging to the specified organisation.</returns>
        Task<IEnumerable<User>> GetUsersByOrganisationAsync(Guid organisationId);

        /// <summary>
        /// Adds a new user to the data store.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to add.</param>
        Task<bool> AddUserAsync(User user);

        /// <summary>
        /// Updates an existing user in the data store.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to update.</param>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user from the data store.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to delete.</param>
        Task<bool> DeleteUserAsync(User user);
    }
}
