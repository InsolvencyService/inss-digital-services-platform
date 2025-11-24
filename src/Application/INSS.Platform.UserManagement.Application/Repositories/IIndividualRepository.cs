using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Repositories;

/// <summary>
/// Defines repository operations for managing <see cref="Individual"/> entities.
/// </summary>
public interface IIndividualRepository : IRepositoryBase<Individual> 
{
    /// <summary>
    /// Retrieves the list of <see cref="Organisation"/> entities associated with a specific individual.
    /// </summary>
    /// <param name="individualId">The unique identifier of the individual.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Organisation"/> entities.
    /// </returns>
    Task<OperationResult<IReadOnlyList<Organisation>>> GetOrganisationsForIndividualAsync(Guid individualId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the list of <see cref="Group"/> entities associated with a specific individual.
    /// </summary>
    /// <param name="individualId">The unique identifier of the individual.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Group"/> entities.
    /// </returns>
    Task<OperationResult<IReadOnlyList<Group>>> GetGroupsForIndividualAsync(Guid individualId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the list of <see cref="Party"/> entities associated with a specific individual.
    /// </summary>
    /// <param name="individualId">The unique identifier of the individual.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Party"/> entities.
    /// </returns>
    Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForIndividualAsync(Guid individualId, CancellationToken cancellationToken);
}
