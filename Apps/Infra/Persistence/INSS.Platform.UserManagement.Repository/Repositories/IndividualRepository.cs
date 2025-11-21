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
    /// Repository for managing <see cref="Individual"/> entities and their related data.
    /// </summary>
    public class IndividualRepository : RepositoryBase<Individual>, IIndividualRepository
    {
        private readonly ILogger<IndividualRepository> _logger;
        private readonly UserManagementDbContext _context;
        private readonly IRelationshipTypeLookupService _relationshipTypeLookupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndividualRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The user management database context.</param>
        /// <param name="relationshipTypeLookupService">The relationship type lookup service.</param>
        public IndividualRepository(
            ILogger<IndividualRepository> logger,
            UserManagementDbContext context,
            IRelationshipTypeLookupService relationshipTypeLookupService) 
            : base(logger, context)
        {
            _logger = logger;
            _context = context;
            _relationshipTypeLookupService = relationshipTypeLookupService;
        }

        /// <summary>
        /// Retrieves an <see cref="Individual"/> entity by its unique identifier, including related <see cref="Party"/>, <see cref="Address"/>, and <see cref="PartyType"/>.
        /// </summary>
        /// <param name="id">The unique identifier of the individual.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the <see cref="Individual"/> if found, or an error result.</returns>
        public override async Task<OperationResult<Individual>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving {EntityType} with ID {EntityId}", nameof(Individual), id);

            try
            {
                Individual? individual = await _context.Individual
                    .Include(i => i.Party)
                    .ThenInclude(p => p!.Addresses)
                    .Include(i => i.Party)
                    .ThenInclude(p => p!.PartyType)
                    .FirstOrDefaultAsync(i => i.Id == id, cancellationToken: cancellationToken);

                if (individual is null)
                {
                    _logger.LogWarning("Entity of type {EntityType} with ID {EntityId} not found", nameof(Individual), id);
                    return Operation.Fail<Individual>($"Entity of type {nameof(Individual)} with ID {id} not found.", OperationErrorCode.NotFound);
                }

                return Operation.Ok(individual);
            }
            catch (SqlException ex)
            {
                return HandlePersistenceException(ex, "retrieving", id);
            }
        }

        /// <summary>
        /// Retrieves the list of <see cref="Organisation"/> entities for which the specified individual is employed.
        /// </summary>
        /// <param name="individualId">The unique identifier of the individual.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing a list of <see cref="Organisation"/> entities, or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Organisation>>> GetOrganisationsForIndividualAsync(Guid individualId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Organisations for Individual with ID: {IndividualId}", individualId);

            const string EmployedBy = "EmployedBy";

            try
            {
                Individual? individual = await _context.Individual.AsNoTracking().FirstOrDefaultAsync(i => i.Id == individualId, cancellationToken);

                if (individual is null)
                {
                    return Operation.Fail<IReadOnlyList<Organisation>>($"Individual with ID {individualId} not found.", OperationErrorCode.NotFound);
                }

                RelationshipType? employedByType = await _relationshipTypeLookupService.GetByNameAsync(EmployedBy, cancellationToken);
                if (employedByType is null)
                {
                    return Operation.Fail<IReadOnlyList<Organisation>>($"RelationshipType '{EmployedBy}' not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> organisationPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == individual.PartyId && r.RelationshipTypeId == employedByType.Id)
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
                string errorMessage = $"An error occurred while retrieving Organisations for Individual with ID {individualId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Organisation>>(errorMessage, OperationErrorCode.SqlError);
            }
        }

        /// <summary>
        /// Retrieves the list of <see cref="Group"/> entities for which the specified individual is a member.
        /// </summary>
        /// <param name="individualId">The unique identifier of the individual.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing a list of <see cref="Group"/> entities, or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Group>>> GetGroupsForIndividualAsync(Guid individualId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Groups for Individual with ID: {IndividualId}", individualId);

            const string MemberOf = "MemberOf";

            try
            {
                Individual? individual = await _context.Individual.AsNoTracking().FirstOrDefaultAsync(i => i.Id == individualId, cancellationToken);

                if (individual is null)
                {
                    return Operation.Fail<IReadOnlyList<Group>>($"Individual with ID {individualId} not found.", OperationErrorCode.NotFound);
                }

                RelationshipType? memberOfType = await _relationshipTypeLookupService.GetByNameAsync(MemberOf, cancellationToken);
                if (memberOfType is null)
                {
                    return Operation.Fail<IReadOnlyList<Group>>($"RelationshipType '{MemberOf}' not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> groupPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == individual.PartyId && r.RelationshipTypeId == memberOfType.Id)
                    .Select(r => r.ToPartyId)
                    .ToListAsync(cancellationToken);

                IReadOnlyList<Group> groups = await _context.Group
                    .Include(o => o.Party)
                    .Where(o => groupPartyIds.Contains(o.PartyId))
                    .ToListAsync(cancellationToken);

                return Operation.Ok(groups);
            }
            catch (SqlException ex)
            {
                string errorMessage = $"An error occurred while retrieving Groups for Individual with ID {individualId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Group>>(errorMessage, OperationErrorCode.SqlError);
            }
        }

        /// <summary>
        /// Retrieves the list of <see cref="Party"/> entities related to the specified individual.
        /// </summary>
        /// <param name="individualId">The unique identifier of the individual.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing a list of <see cref="Party"/> entities, or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForIndividualAsync(Guid individualId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Parties for Individual with ID: {IndividualId}", individualId);

            try
            {
                Individual? individual = await _context.Individual.AsNoTracking().FirstOrDefaultAsync(i => i.Id == individualId, cancellationToken);

                if (individual is null)
                {
                    return Operation.Fail<IReadOnlyList<Party>>($"Individual with ID {individualId} not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> individualPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == individual.PartyId)
                    .Select(r => r.ToPartyId)
                    .ToListAsync(cancellationToken);

                IReadOnlyList<Party> parties = await _context.Party
                    .Where(i => individualPartyIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                return Operation.Ok(parties);
            }
            catch (SqlException ex)
            {
                string errorMessage = $"An error occurred while retrieving Parties for Individual with ID {individualId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Party>>(errorMessage, OperationErrorCode.SqlError);
            }
        }
    }
}
