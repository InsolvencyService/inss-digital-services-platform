using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Services
{
    /// <summary>
    /// Provides methods for looking up relationship types.
    /// </summary>
    public interface IRelationshipTypeLookupService
    {
        /// <summary>
        /// Retrieves a <see cref="RelationshipType"/> by its name asynchronously.
        /// </summary>
        /// <param name="name">The name of the relationship type to retrieve.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the <see cref="RelationshipType"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<RelationshipType?> GetByNameAsync(string name, CancellationToken cancellationToken);
    }
}
