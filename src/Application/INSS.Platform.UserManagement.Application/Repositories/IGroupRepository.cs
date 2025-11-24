using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Repositories;

/// <summary>
/// Defines repository operations for managing <see cref="Group"/> entities and their related associations.
/// </summary>
public interface IGroupRepository : IRepositoryBase<Group> 
{
    /// <summary>
    /// Retrieves the list of <see cref="Organisation"/> entities associated with the specified group.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Organisation"/> entities.
    /// </returns>
    Task<OperationResult<IReadOnlyList<Organisation>>> GetOrganisationsForGroupAsync(Guid groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the list of <see cref="Individual"/> entities associated with the specified group.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Individual"/> entities.
    /// </returns>
    Task<OperationResult<IReadOnlyList<Individual>>> GetIndividualsForGroupAsync(Guid groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the list of <see cref="Party"/> entities associated with the specified group.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing a read-only list of <see cref="Party"/> entities.
    /// </returns>
    Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForGroupAsync(Guid groupId, CancellationToken cancellationToken);
}
