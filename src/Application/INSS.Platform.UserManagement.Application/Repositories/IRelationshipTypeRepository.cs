using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Repositories;

/// <summary>
/// Defines repository operations for <see cref="RelationshipType"/> entities.
/// </summary>
public interface IRelationshipTypeRepository : IRepositoryBase<RelationshipType>
{
    /// <summary>
    /// Asynchronously retrieves a <see cref="RelationshipType"/> entity by its name.
    /// </summary>
    /// <param name="name">The name of the relationship type to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="OperationResult{T}"/> containing the <see cref="RelationshipType"/> if found, or an error result otherwise.
    /// </returns>
    Task<OperationResult<RelationshipType>> GetByNameAsync(string name, CancellationToken cancellationToken);
}
