using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.Canonical.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="User"/> entities.
/// </summary>
public class UserRepository : RepositoryBase<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly CanonicalDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for user management.</param>
    public UserRepository(ILogger<UserRepository> logger, CanonicalDbContext context) 
        : base(logger, context) 
    { 
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Retrieves a <see cref="User"/> entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the user if found, or an error result.</returns>
    public override async Task<OperationResult<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving {EntityType} with ID {EntityId}", nameof(User), id);

        try
        {
            User? user = await _context.User
                .Include(user => user.Addresses)
                .Include(user => user.BankDetails)
                .Include(user => user.Incomes)
                .FirstOrDefaultAsync(user => user.Id == id, cancellationToken: cancellationToken);

            if (user is null)
            {
                _logger.LogWarning("Entity of type {EntityType} with ID {EntityId} not found", nameof(User), id);
                return Operation.Fail<User>($"Entity of type {nameof(User)} with ID {id} not found.", OperationErrorCode.NotFound);
            }

            return Operation.Ok(user);
        }
        catch (SqlException ex)
        {
            return HandlePersistenceException(ex, "retrieving", id);
        }
    }
}
