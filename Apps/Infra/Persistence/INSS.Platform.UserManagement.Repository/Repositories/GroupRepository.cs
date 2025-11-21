using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Abstractions.Services;
using INSS.Platform.UserManagement.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace INSS.Platform.UserManagement.Repository.Repositories
{
    /// <summary>
    /// Repository for managing <see cref="Group"/> entities and related operations.
    /// </summary>
    public class GroupRepository : RepositoryBase<Group>, IGroupRepository
    {
        private readonly ILogger<GroupRepository> _logger;
        private readonly UserManagementDbContext _context;
        private readonly IRelationshipTypeLookupService _relationshipTypeLookupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The database context.</param>
        /// <param name="relationshipTypeLookupService">The relationship type lookup service.</param>
        public GroupRepository(ILogger<GroupRepository> logger, UserManagementDbContext context, IRelationshipTypeLookupService relationshipTypeLookupService)
            : base(logger, context) 
        {
            _logger = logger;
            _context = context;
            _relationshipTypeLookupService = relationshipTypeLookupService;
        }

        /// <summary>
        /// Retrieves a <see cref="Group"/> entity by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the group.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the group if found, or an error result.</returns>
        public override async Task<OperationResult<Group>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving {EntityType} with ID {EntityId}", nameof(Group), id);

            try
            {
                Group? group = await _context.Group
                    .Include(i => i.Party)
                    .ThenInclude(p => p!.PartyType)
                    .FirstOrDefaultAsync(i => i.Id == id, cancellationToken: cancellationToken);

                if (group is null)
                {
                    _logger.LogWarning("Entity of type {EntityType} with ID {EntityId} not found", nameof(Group), id);
                    return Operation.Fail<Group>($"Entity of type {nameof(Group)} with ID {id} not found.", OperationErrorCode.NotFound);
                }

                return Operation.Ok(group);
            }
            catch (SqlException ex)
            {
                return HandlePersistenceException(ex, "retrieving", id);
            }
        }

        /// <summary>
        /// Retrieves the list of <see cref="Individual"/> members for a specified group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the list of individuals or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Individual>>> GetIndividualsForGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Individual for Group with ID: {GroupId}", groupId);

            const string HasMember = "HasMember";

            try
            {
                Group? group = await _context.Group.AsNoTracking().FirstOrDefaultAsync(i => i.Id == groupId, cancellationToken);

                if (group is null)
                {
                    return Operation.Fail<IReadOnlyList<Individual>>($"Group with ID {groupId} not found.", OperationErrorCode.NotFound);
                }

                RelationshipType? hasMemberType = await _relationshipTypeLookupService.GetByNameAsync(HasMember, cancellationToken);
                if (hasMemberType is null)
                {
                    return Operation.Fail<IReadOnlyList<Individual>>($"RelationshipType '{HasMember}' not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> individualPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == group.PartyId && r.RelationshipTypeId == hasMemberType.Id)
                    .Select(r => r.ToPartyId)
                    .ToListAsync(cancellationToken);

                IReadOnlyList<Individual> individuals = await _context.Individual
                    .Include(o => o.Party)
                    .Where(o => individualPartyIds.Contains(o.PartyId))
                    .ToListAsync(cancellationToken);

                return Operation.Ok(individuals);
            }
            catch (SqlException ex)
            {
                string errorMessage = $"An error occurred while retrieving Individuals for Group with ID {groupId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Individual>>(errorMessage, OperationErrorCode.SqlError);
            }
        }

        /// <summary>
        /// Retrieves the list of <see cref="Organisation"/> entities related to a specified group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the list of organisations or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Organisation>>> GetOrganisationsForGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Organisations for Group with ID: {GroupId}", groupId);

            const string EmployedBy = "EmployedBy";

            try
            {
                Group? group = await _context.Group.AsNoTracking().FirstOrDefaultAsync(i => i.Id == groupId, cancellationToken);

                if (group is null)
                {
                    return Operation.Fail<IReadOnlyList<Organisation>>($"Group with ID {groupId} not found.", OperationErrorCode.NotFound);
                }

                RelationshipType? employedByType = await _relationshipTypeLookupService.GetByNameAsync(EmployedBy, cancellationToken);
                if (employedByType is null)
                {
                    return Operation.Fail<IReadOnlyList<Organisation>>($"RelationshipType '{EmployedBy}' not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> organisationPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == group.PartyId && r.RelationshipTypeId == employedByType.Id)
                    .Select(r => r.ToPartyId)
                    .ToListAsync(cancellationToken);

                IReadOnlyList<Organisation> organisations = await _context.Organisation
                    .Include(o => o.Party)
                    .Where(o => organisationPartyIds.Contains(o.PartyId))
                    .ToListAsync(cancellationToken);

                return Operation.Ok(organisations);
            }
            catch (SqlException ex)
            {
                string errorMessage = $"An error occurred while retrieving Organisations for Group with ID {groupId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Organisation>>(errorMessage, OperationErrorCode.SqlError);
            }
        }

        /// <summary>
        /// Retrieves the list of <see cref="Party"/> entities related to a specified group.
        /// </summary>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the list of parties or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForGroupAsync(Guid groupId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Parties for Group with ID: {GroupId}", groupId);

            try
            {
                Group? group = await _context.Group.AsNoTracking().FirstOrDefaultAsync(i => i.Id == groupId, cancellationToken);

                if (group is null)
                {
                    return Operation.Fail<IReadOnlyList<Party>>($"Group with ID {groupId} not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> groupPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == group.PartyId)
                    .Select(r => r.ToPartyId)
                    .ToListAsync(cancellationToken);

                IReadOnlyList<Party> parties = await _context.Party
                    .Where(i => groupPartyIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                return Operation.Ok(parties);
            }
            catch (SqlException ex)
            {
                string errorMessage = $"An error occurred while retrieving Parties for Group with ID {groupId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Party>>(errorMessage, OperationErrorCode.SqlError);
            }
        }
    }
}
