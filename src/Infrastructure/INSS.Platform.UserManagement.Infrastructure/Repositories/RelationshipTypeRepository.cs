using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

/// <summary>
/// Repository for managing <see cref="RelationshipType"/> entities.
/// </summary>
public class RelationshipTypeRepository : RepositoryBase<RelationshipType>, IRelationshipTypeRepository
{
    private readonly ILogger<RelationshipTypeRepository> _logger;
    private readonly UserManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationshipTypeRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="context">The user management database context.</param>
    public RelationshipTypeRepository(ILogger<RelationshipTypeRepository> logger, UserManagementDbContext context) : base(logger, context) 
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Retrieves a <see cref="RelationshipType"/> entity by its name.
    /// </summary>
    /// <param name="name">The name of the relationship type to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>
    /// An <see cref="OperationResult{RelationshipType}"/> containing the result of the operation.
    /// If the entity is found, the result is successful; otherwise, it contains an error.
    /// </returns>
    public async Task<OperationResult<RelationshipType>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving RelationshipType with Name: {RelationshipTypeName}", name);

        try
        {
            RelationshipType? relationshipType = await _context.RelationshipType
                .FirstOrDefaultAsync(rt => rt.Name == name, cancellationToken);

            if (relationshipType is null)
            {
                return Operation.Fail<RelationshipType>($"Entity of type {nameof(RelationshipType)} with Name {name} not found.", OperationErrorCode.NotFound);
            }

            return Operation.Ok(relationshipType);
        }
        catch (SqlException ex)
        {
            return HandlePersistenceException(ex, "retrieving", name: name);
        }
    }
}
