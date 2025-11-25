using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;

namespace INSS.Platform.UserManagement.Application.Repositories;

/// <summary>
/// Defines a generic repository interface for basic CRUD operations and exception handling.
/// </summary>
/// <typeparam name="T">The type of entity managed by the repository, must inherit from <see cref="AuditableEntity"/>.</typeparam>
public interface IRepositoryBase<T> where T : AuditableEntity
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the entity if found, or an error result.</returns>
    Task<OperationResult<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{T}}"/> containing the collection of entities, or an error result.</returns>
    Task<OperationResult<IEnumerable<T>>> GetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the added entity, or an error result.</returns>
    Task<OperationResult<T>> AddAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the updated entity, or an error result.</returns>
    Task<OperationResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the deleted entity, or an error result.</returns>
    Task<OperationResult<T>> DeleteAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Handles exceptions that occur during persistence operations for a single entity.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="action">The action being performed when the exception occurred.</param>
    /// <param name="id">The unique identifier of the entity involved, if applicable.</param>
    /// <param name="name">The name of the entity involved, if applicable.</param>
    /// <returns>An <see cref="OperationResult{T}"/> representing the error.</returns>
    OperationResult<T> HandlePersistenceException(Exception ex, string action, Guid? id = null, string? name = null);

    /// <summary>
    /// Handles exceptions that occur during persistence operations for multiple entities.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    /// <param name="action">The action being performed when the exception occurred.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{T}}"/> representing the error.</returns>
    OperationResult<IEnumerable<T>> HandlePersistenceException(Exception ex, string action);
}
