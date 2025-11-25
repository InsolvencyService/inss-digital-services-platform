using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Infrastructure.Repositories;

/// <summary>
/// Repository for managing <see cref="Party"/> entities and related operations.
/// </summary>
public class PartyRepository : RepositoryBase<Party>, IPartyRepository
{
    private readonly ILogger<PartyRepository> _logger;
    private readonly UserManagementDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartyRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="context">The user management database context.</param>
    public PartyRepository(ILogger<PartyRepository> logger, UserManagementDbContext context) : base(logger, context) 
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Retrieves a <see cref="Party"/> entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the party.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{Party}"/> containing the party if found, or an error result.</returns>
    public override async Task<OperationResult<Party>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving {EntityType} with ID {EntityId}", nameof(Party), id);

        try
        {
            Party? party = await _context.Party
                .Include(p => p.Addresses)
                .Include(p => p.PartyType)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken: cancellationToken);

            if (party is null)
            {
                _logger.LogWarning("Entity of type {EntityType} with ID {EntityId} not found", nameof(Party), id);
                return Operation.Fail<Party>($"Entity of type {nameof(Party)} with ID {id} not found.", OperationErrorCode.NotFound);
            }

            return Operation.Ok(party);
        }
        catch (SqlException ex)
        {
            return HandlePersistenceException(ex, "retrieving", id);
        }
    }

    /// <summary>
    /// Retrieves all <see cref="Party"/> entities.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{IEnumerable{Party}}"/> containing the list of parties or an error result.</returns>
    public override async Task<OperationResult<IEnumerable<Party>>> GetAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving all entities of type {EntityType}", nameof(Party));

        try
        {
            IEnumerable<Party> parties = await _context.Party
                .Include(p => p.Addresses)
                .Include(p => p.PartyType)
                .ToListAsync(cancellationToken);

            return Operation.Ok(parties);
        }
        catch (SqlException ex)
        {
            return HandlePersistenceException(ex, "retrieving");
        }
    }

    /// <summary>
    /// Retrieves all <see cref="Group"/> entities related to a specific <see cref="Party"/>.
    /// </summary>
    /// <param name="partyId">The unique identifier of the party.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{IReadOnlyList{Group}}"/> containing the list of groups or an error result.</returns>
    public async Task<OperationResult<IReadOnlyList<Group>>> GetGroupsForPartyAsync(Guid partyId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Groups for Party with ID: {PartyId}", partyId);

        try
        {
            Party? party = await _context.Party.AsNoTracking().FirstOrDefaultAsync(i => i.Id == partyId, cancellationToken);

            if (party is null)
            {
                return Operation.Fail<IReadOnlyList<Group>>($"Party with ID {partyId} not found.", OperationErrorCode.NotFound);
            }

            IReadOnlyList<Guid> groupIds = await _context.PartyRelationship
                .Where(r => r.FromPartyId == party.Id)
                .Select(r => r.ToPartyId)
                .ToListAsync(cancellationToken);

            IReadOnlyList<Group> groups = await _context.Group
                .Where(i => groupIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            return Operation.Ok(groups);
        }
        catch (SqlException ex)
        {
            string errorMessage = $"An error occurred while retrieving Groups for Party with ID {partyId}.";
            _logger.LogError(ex, "{ErrorMessage}", errorMessage);
            return Operation.Fail<IReadOnlyList<Group>>(errorMessage, OperationErrorCode.SqlError);
        }
    }

    /// <summary>
    /// Retrieves all <see cref="Individual"/> entities related to a specific <see cref="Party"/>.
    /// </summary>
    /// <param name="partyId">The unique identifier of the party.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{IReadOnlyList{Individual}}"/> containing the list of individuals or an error result.</returns>
    public async Task<OperationResult<IReadOnlyList<Individual>>> GetIndividualsForPartyAsync(Guid partyId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Individuals for Party with ID: {PartyId}", partyId);
        try
        {
            Party? party = await _context.Party.AsNoTracking().FirstOrDefaultAsync(i => i.Id == partyId, cancellationToken);

            if (party is null)
            {
                return Operation.Fail<IReadOnlyList<Individual>>($"Party with ID {partyId} not found.", OperationErrorCode.NotFound);
            }

            IReadOnlyList<Guid> individualIds = await _context.PartyRelationship
                .Where(r => r.FromPartyId == party.Id)
                .Select(r => r.ToPartyId)
                .ToListAsync(cancellationToken);

            IReadOnlyList<Individual> individuals = await _context.Individual
                .Where(i => individualIds.Contains(i.Id))
                .ToListAsync(cancellationToken);
            
            return Operation.Ok(individuals);
        }
        catch (SqlException ex)
        {
            string errorMessage = $"An error occurred while retrieving Individuals for Party with ID {partyId}.";
            _logger.LogError(ex, "{ErrorMessage}", errorMessage);
            return Operation.Fail<IReadOnlyList<Individual>>(errorMessage, OperationErrorCode.SqlError);
        }
    }

    /// <summary>
    /// Retrieves all <see cref="Organisation"/> entities related to a specific <see cref="Party"/>.
    /// </summary>
    /// <param name="partyId">The unique identifier of the party.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{IReadOnlyList{Organisation}}"/> containing the list of organisations or an error result.</returns>
    public async Task<OperationResult<IReadOnlyList<Organisation>>> GetOrganisationsForPartyAsync(Guid partyId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Organisations for Party with ID: {PartyId}", partyId);

        try
        {
            Party? party = await _context.Party.AsNoTracking().FirstOrDefaultAsync(i => i.Id == partyId, cancellationToken);

            if (party is null)
            {
                return Operation.Fail<IReadOnlyList<Organisation>>($"Party with ID {partyId} not found.", OperationErrorCode.NotFound);
            }

            IReadOnlyList<Guid> organisationIds = await _context.PartyRelationship
                .Where(r => r.FromPartyId == party.Id)
                .Select(r => r.ToPartyId)
                .ToListAsync(cancellationToken);

            IReadOnlyList<Organisation> organisations = await _context.Organisation
                .Where(i => organisationIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            return Operation.Ok(organisations);
        }
        catch (SqlException ex)
        {
            string errorMessage = $"An error occurred while retrieving Organisations for Party with ID {partyId}.";
            _logger.LogError(ex, "{ErrorMessage}", errorMessage);
            return Operation.Fail<IReadOnlyList<Organisation>>(errorMessage, OperationErrorCode.SqlError);
        }
    }

    /// <summary>
    /// Retrieves all <see cref="Party"/> entities related to a specific <see cref="Party"/>.
    /// </summary>
    /// <param name="partyId">The unique identifier of the party.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="OperationResult{IReadOnlyList{Party}}"/> containing the list of related parties or an error result.</returns>
    public async Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForPartyAsync(Guid partyId, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Retrieving Parties for Party with ID: {PartyId}", partyId);

        try
        {
            Party? party = await _context.Party.AsNoTracking().FirstOrDefaultAsync(i => i.Id == partyId, cancellationToken);

            if (party is null)
            {
                return Operation.Fail<IReadOnlyList<Party>>($"Party with ID {partyId} not found.", OperationErrorCode.NotFound);
            }

            IReadOnlyList<Guid> partyIds = await _context.PartyRelationship
                .Where(r => r.FromPartyId == party.Id)
                .Select(r => r.ToPartyId)
                .ToListAsync(cancellationToken);

            IReadOnlyList<Party> parties = await _context.Party
                .Where(i => partyIds.Contains(i.Id))
                .ToListAsync(cancellationToken);

            return Operation.Ok(parties);
        }
        catch (SqlException ex)
        {
            string errorMessage = $"An error occurred while retrieving Parties for Party with ID {partyId}.";
            _logger.LogError(ex, "{ErrorMessage}", errorMessage);
            return Operation.Fail<IReadOnlyList<Party>>(errorMessage, OperationErrorCode.SqlError);
        }
    }
}
