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
    /// Repository for managing <see cref="Organisation"/> entities and related queries.
    /// </summary>
    public class OrganisationRepository : RepositoryBase<Organisation>, IOrganisationRepository
    {
        private readonly ILogger<OrganisationRepository> _logger;
        private readonly UserManagementDbContext _context;
        private readonly IRelationshipTypeLookupService _relationshipTypeLookupService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="context">The user management database context.</param>
        /// <param name="relationshipTypeLookupService">The relationship type lookup service.</param>
        public OrganisationRepository(ILogger<OrganisationRepository> logger, UserManagementDbContext context, IRelationshipTypeLookupService relationshipTypeLookupService)
            : base(logger, context)
        {
            _logger = logger;
            _context = context;
            _relationshipTypeLookupService = relationshipTypeLookupService;
        }

        /// <summary>
        /// Retrieves an <see cref="Organisation"/> by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{Organisation}"/> containing the organisation if found, or an error result.</returns>
        public override async Task<OperationResult<Organisation>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving {EntityType} with ID {EntityId}", nameof(Organisation), id);

            try
            {
                Organisation? organisation = await _context.Organisation
                    .Include(i => i.Party)
                    .ThenInclude(p => p!.Addresses)
                    .Include(i => i.Party)
                    .ThenInclude(p => p!.PartyType)
                    .FirstOrDefaultAsync(i => i.Id == id, cancellationToken: cancellationToken);

                if (organisation is null)
                {
                    _logger.LogWarning("Entity of type {EntityType} with ID {EntityId} not found", nameof(Organisation), id);
                    return Operation.Fail<Organisation>($"Entity of type {nameof(Organisation)} with ID {id} not found.", OperationErrorCode.NotFound);
                }

                return Operation.Ok(organisation);
            }
            catch (SqlException ex)
            {
                return HandlePersistenceException(ex, "retrieving", id);
            }
        }

        /// <summary>
        /// Retrieves the groups associated with a given organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{IReadOnlyList{Group}}"/> containing the list of groups or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Group>>> GetGroupsForOrganisationAsync(Guid organisationId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Groups for Organisation with ID: {OrganisationId}", organisationId);

            const string GroupRelationshipTypeName = "MemberOf";

            try
            {
                Organisation? organisation = await _context.Organisation.AsNoTracking().FirstOrDefaultAsync(i => i.Id == organisationId, cancellationToken);

                if (organisation is null)
                {
                    return Operation.Fail<IReadOnlyList<Group>>($"Organisation with ID {organisationId} not found.", OperationErrorCode.NotFound);
                }

                RelationshipType? memberOf = await _relationshipTypeLookupService.GetByNameAsync(GroupRelationshipTypeName, cancellationToken);
                if (memberOf is null)
                {
                    return Operation.Fail<IReadOnlyList<Group>>($"RelationshipType '{GroupRelationshipTypeName}' not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> groupPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == organisation.PartyId && r.RelationshipTypeId == memberOf.Id)
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
                string errorMessage = $"An error occurred while retrieving Groups for Organisation with ID {organisationId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Group>>(errorMessage, OperationErrorCode.SqlError);
            }
        }

        /// <summary>
        /// Retrieves the individuals associated with a given organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{IReadOnlyList{Individual}}"/> containing the list of individuals or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Individual>>> GetIndividualsForOrganisationAsync(Guid organisationId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Individual for Organisation with ID: {OrganisationId}", organisationId);

            const string Employs = "Employs";

            try
            {
                Organisation? organisation = await _context.Organisation.AsNoTracking().FirstOrDefaultAsync(i => i.Id == organisationId, cancellationToken);

                if (organisation is null)
                {
                    return Operation.Fail<IReadOnlyList<Individual>>($"Organisation with ID {organisationId} not found.", OperationErrorCode.NotFound);
                }

                RelationshipType? employsType = await _relationshipTypeLookupService.GetByNameAsync(Employs, cancellationToken);
                if (employsType is null)
                {
                    return Operation.Fail<IReadOnlyList<Individual>>($"RelationshipType '{Employs}' not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> individualPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == organisation.PartyId && r.RelationshipTypeId == employsType.Id)
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
                string errorMessage = $"An error occurred while retrieving Individuals for Organisation with ID {organisationId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Individual>>(errorMessage, OperationErrorCode.SqlError);
            }
        }

        /// <summary>
        /// Retrieves the parties associated with a given organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An <see cref="OperationResult{IReadOnlyList{Party}}"/> containing the list of parties or an error result.</returns>
        public async Task<OperationResult<IReadOnlyList<Party>>> GetPartiesForOrganisationAsync(Guid organisationId, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Retrieving Parties for Organisation with ID: {GroupId}", organisationId);

            try
            {
                Organisation? organisation = await _context.Organisation.AsNoTracking().FirstOrDefaultAsync(i => i.Id == organisationId, cancellationToken);

                if (organisation is null)
                {
                    return Operation.Fail<IReadOnlyList<Party>>($"Organisation with ID {organisationId} not found.", OperationErrorCode.NotFound);
                }

                IReadOnlyList<Guid> organisationPartyIds = await _context.PartyRelationship
                    .Where(r => r.FromPartyId == organisation.PartyId)
                    .Select(r => r.ToPartyId)
                    .ToListAsync(cancellationToken);

                IReadOnlyList<Party> parties = await _context.Party
                    .Where(i => organisationPartyIds.Contains(i.Id))
                    .ToListAsync(cancellationToken);

                return Operation.Ok(parties);
            }
            catch (SqlException ex)
            {
                string errorMessage = $"An error occurred while retrieving Parties for Organisation with ID {organisationId}.";
                _logger.LogError(ex, "{ErrorMessage}", errorMessage);
                return Operation.Fail<IReadOnlyList<Party>>(errorMessage, OperationErrorCode.SqlError);
            }
        }
    }
}
