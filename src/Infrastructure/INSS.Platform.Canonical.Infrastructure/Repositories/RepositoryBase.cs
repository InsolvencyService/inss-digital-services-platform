using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.Canonical.Infrastructure.Repositories;

/// <summary>
/// Provides a base implementation for repository operations on entities of type <typeparamref name="T"/>.
/// Handles common CRUD operations and persistence exception handling.
/// </summary>
/// <typeparam name="T">The entity type, which must inherit from <see cref="BaseEntity"/>.</typeparam>
public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity
{
    private readonly CanonicalDbContext _context;
    private readonly ILogger<RepositoryBase<T>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryBase{T}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="context">The database context.</param>
    protected RepositoryBase(ILogger<RepositoryBase<T>> logger, CanonicalDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the entity if found, or an error result.</returns>
    public virtual async Task<OperationResult<T>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving entity of type {EntityType} with ID {EntityId}", typeof(T).Name, id);

        try
        {
            T? entity = await _context.Set<T>().FindAsync([id], cancellationToken);
            
            if (entity == null)
            {
                _logger.LogWarning("Entity of type {EntityType} with ID {EntityId} not found", typeof(T).Name, id);
                return Operation.Fail<T>($"Entity of type {typeof(T).Name} with ID {id} not found.", OperationErrorCode.NotFound);
            }

            return Operation.Ok(entity);
        }
        catch (SqlException ex)
        {
            return HandlePersistenceException(ex, "retrieving", id);
        }
    }

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{T}}"/> containing the entities or an error result.</returns>
    public virtual async Task<OperationResult<IEnumerable<T>>> GetAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all entities of type {EntityType}", typeof(T).Name);

        try
        {
            IEnumerable<T> entities = await _context.Set<T>().ToListAsync(cancellationToken);
            return Operation.Ok(entities);
        }
        catch (SqlException ex)
        {
            return HandlePersistenceException(ex, "retrieving");
        }
    }

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the added entity or an error result.</returns>
    public virtual async Task<OperationResult<T>> AddAsync(T entity, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Adding new entity of type {EntityType}", typeof(T).Name);

        try
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Operation.Ok(entity);
        }
        catch(Exception ex) when (ex is SqlException or DbUpdateException)
        {
            return HandlePersistenceException(ex, "adding", entity.Id);
        }
    }

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the updated entity or an error result.</returns>
    public virtual async Task<OperationResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Updating entity of type {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);

        try
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return Operation.Ok(entity);
        }
        catch (Exception ex) when (ex is SqlException or DbUpdateException)
        {
            return HandlePersistenceException(ex, "updating", entity.Id);
        }
    }

    /// <summary>
    /// Deletes an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the deleted entity or an error result.</returns>
    public virtual async Task<OperationResult<T>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Deleting entity of type {EntityType} with ID {EntityId}", typeof(T).Name, id);

        try
        {
            OperationResult<T> result = await GetByIdAsync(id, cancellationToken);

            if (!result.Success)
            {
                return result;
            }

            _context.Set<T>().Remove(result.Entity!);
            await _context.SaveChangesAsync(cancellationToken);

            return Operation.Ok(result.Entity);
        }
        catch (Exception ex) when (ex is SqlException or DbUpdateException)
        {
            return HandlePersistenceException(ex, "deleting", id);
        }
    }

    /// <summary>
    /// Handles persistence exceptions for single entity operations.
    /// </summary>
    /// <param name="ex">The exception thrown.</param>
    /// <param name="action">The action being performed.</param>
    /// <param name="id">The entity identifier, if available.</param>
    /// <param name="name">The entity name, if available.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the error details.</returns>
    public OperationResult<T> HandlePersistenceException(Exception ex, string action, Guid? id = null, string? name = null)
          => HandlePersistenceException<T>(ex, action, id, name);

    /// <summary>
    /// Handles persistence exceptions for collection operations.
    /// </summary>
    /// <param name="ex">The exception thrown.</param>
    /// <param name="action">The action being performed.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{T}}"/> containing the error details.</returns>
    public OperationResult<IEnumerable<T>> HandlePersistenceException(Exception ex, string action)
        => HandlePersistenceException<IEnumerable<T>>(ex, action);

    /// <summary>
    /// Handles persistence exceptions and builds an appropriate error result.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="ex">The exception thrown.</param>
    /// <param name="action">The action being performed.</param>
    /// <param name="id">The entity identifier, if available.</param>
    /// <param name="name">The entity name, if available.</param>
    /// <returns>An <see cref="OperationResult{TResult}"/> containing the error details.</returns>
    private OperationResult<TResult> HandlePersistenceException<TResult>(Exception ex, string action, Guid? id = null, string? name = null)
    {
        string errorMessage = BuildErrorMessage(ex, action, id, name);
        _logger.LogError(ex, "{ErrorMessage}", errorMessage);
        return Operation.Fail<TResult>(errorMessage, OperationErrorCode.SqlError);
    }

    /// <summary>
    /// Builds a detailed error message for persistence exceptions.
    /// </summary>
    /// <param name="ex">The exception thrown.</param>
    /// <param name="action">The action being performed.</param>
    /// <param name="id">The entity identifier, if available.</param>
    /// <param name="name">The entity name, if available.</param>
    /// <returns>A detailed error message string.</returns>
    private static string BuildErrorMessage(Exception ex, string action, Guid? id = null, string? name = null)
    {
        SqlException? sqlEx = ex switch
        {
            SqlException direct => direct,
            DbUpdateException db when db.InnerException is SqlException inner => inner,
            _ => null
        };

        string baseMessage = sqlEx != null
            ? sqlEx.Number switch
            {
                547 => "Foreign key violation occurred.",
                2601 or 2627 => "Unique constraint violation occurred.",
                _ => $"A database error occurred. Error details: {sqlEx.Message}"
            }
            : ex is DbUpdateException dbUpdateEx
                ? $"A database update error occurred. Error details: {dbUpdateEx.Message}"
                : $"An unknown database error occurred. Error details: {ex.Message}";

        string details = $" While {action} entity of type {typeof(T).Name}.";
        if (id.HasValue)
        {
            details += $" Entity ID: {id.Value}.";
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            details += $" Entity Name: {name}.";
        }

        return baseMessage + details;
    }
}
