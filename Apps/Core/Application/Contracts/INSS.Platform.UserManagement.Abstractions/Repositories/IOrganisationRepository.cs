using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Repositories
{
    /// <summary>
    /// Defines repository operations for managing <see cref="Organisation"/> entities and related data.
    /// </summary>
    public interface IOrganisationRepository : IRepositoryBase<Organisation> 
    {
        /// <summary>
        /// Retrieves the list of <see cref="Individual"/>s associated with the specified organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Individual"/>s.
        /// </returns>
        Task<OperationResult<IReadOnlyList<Individual>>> GetIndividualsForOrganisationAsync(Guid organisationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the list of <see cref="Group"/>s associated with the specified organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Group"/>s.
        /// </returns>
        Task<OperationResult<IReadOnlyList<Group>>> GetGroupsForOrganisationAsync(Guid organisationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the list of <see cref="Party"/>s associated with the specified organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Party"/>s.
        /// </returns>
        Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForOrganisationAsync(Guid organisationId, CancellationToken cancellationToken);
    }
}
