using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;

namespace INSS.Platform.UserManagement.Abstractions.Repositories
{
    /// <summary>
    /// Defines repository operations for managing <see cref="Party"/> entities and their related types.
    /// </summary>
    public interface IPartyRepository : IRepositoryBase<Party> 
    {
        /// <summary>
        /// Retrieves the list of <see cref="Individual"/> entities associated with the specified party.
        /// </summary>
        /// <param name="partyId">The unique identifier of the party.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Individual"/> entities.
        /// </returns>
        Task<OperationResult<IReadOnlyList<Individual>>> GetIndividualsForPartyAsync(Guid partyId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the list of <see cref="Group"/> entities associated with the specified party.
        /// </summary>
        /// <param name="partyId">The unique identifier of the party.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Group"/> entities.
        /// </returns>
        Task<OperationResult<IReadOnlyList<Group>>> GetGroupsForPartyAsync(Guid partyId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the list of <see cref="Organisation"/> entities associated with the specified party.
        /// </summary>
        /// <param name="partyId">The unique identifier of the party.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Organisation"/> entities.
        /// </returns>
        Task<OperationResult<IReadOnlyList<Organisation>>> GetOrganisationsForPartyAsync(Guid partyId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves the list of <see cref="Party"/> entities associated with the specified party.
        /// </summary>
        /// <param name="partyId">The unique identifier of the party.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>
        /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Party"/> entities.
        /// </returns>
        Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForPartyAsync(Guid partyId, CancellationToken cancellationToken);
    }
}
