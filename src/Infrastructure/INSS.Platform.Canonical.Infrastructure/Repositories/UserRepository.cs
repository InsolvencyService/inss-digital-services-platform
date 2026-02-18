using INSS.Platform.Audit.Application.Events;
using INSS.Platform.Audit.Application.Users.Commands;
using INSS.Platform.Audit.Application.Users.Handlers;
using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using INSS.Platform.Events.Domain;
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
    private readonly IDomainEventDispatcher _dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging repository operations.</param>
    /// <param name="context">The database context for accessing user data.</param>
    /// <param name="dispatcher">The domain event dispatcher for publishing domain events.</param>
    public UserRepository(ILogger<UserRepository> logger, CanonicalDbContext context, IDomainEventDispatcher dispatcher) 
        : base(logger, context) 
    { 
        _logger = logger;
        _context = context;
        _dispatcher = dispatcher;
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

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="user">The user to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{T}"/> containing the added user or an error result.</returns>
    public override async Task<OperationResult<User>> AddAsync(User user, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Adding new entity of type {EntityType}", nameof(User));

        try
        {
            _context.Set<User>().Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            await RaiseAuditEventsAsync(user, Guid.NewGuid(), user.CreatedBy ?? "Not Set", cancellationToken);

            return Operation.Ok(user);
        }
        catch (Exception ex) when (ex is SqlException or DbUpdateException)
        {
            return HandlePersistenceException(ex, "adding", user.Id);
        }
    }

    /// <summary>
    /// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
    /// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
    /// In a properly defined application the events would be documented and also adhere to a defined contract.
    /// </summary>
    private async Task RaiseAuditEventsAsync(User user, Guid instanceId, string currentUser, CancellationToken cancellationToken)
    {
        AddUserDetailsCommand userDetailsCommand = new()
        {
            User = currentUser,
            CorrelationId = instanceId,
            FullName = user.FullName,
            DateOfBirth = user.DateOfBirth,
            TelephoneNumber = user.TelephoneNumber,
            EmailAddress = user.EmailAddress
        };

        AddUserDetailsHandler.Handle(user, userDetailsCommand);

        foreach (Income income in user.Incomes)
        {
            {
                AddUserIncomeCommand incomeCommand = new()
                {
                    User = currentUser,
                    CorrelationId = instanceId,
                    GrossIncome = income.GrossIncome,
                    IncomeProvider = income.IncomeProvider
                };

                AddUserIncomeHandler.Handle(user, incomeCommand);
            }

        }

        List<IDomainEvent> events = [.. user.DomainEvents];
        user.ClearDomainEvents();

        await _dispatcher.DispatchAsync(events, cancellationToken);
    }
}
