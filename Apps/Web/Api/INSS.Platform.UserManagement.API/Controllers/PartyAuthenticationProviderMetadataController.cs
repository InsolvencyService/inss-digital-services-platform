using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Party Authentication Provider Metadata.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class PartyAuthenticationProviderMetadataController : ControllerBase
    {
        private readonly ILogger<PartyAuthenticationProviderMetadataController> _logger;
        private readonly IPartyAuthenticationProviderMetadataRepository _partyAuthenticationProviderMetadataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyAuthenticationProviderMetadataController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging operations.</param>
        /// <param name="partyAuthenticationProviderMetadataRepository">Repository for party authentication provider metadata.</param>
        public PartyAuthenticationProviderMetadataController(
            ILogger<PartyAuthenticationProviderMetadataController> logger,
            IPartyAuthenticationProviderMetadataRepository partyAuthenticationProviderMetadataRepository)
        {
            _logger = logger;
            _partyAuthenticationProviderMetadataRepository = partyAuthenticationProviderMetadataRepository;
        }

        /// <summary>
        /// Gets a specific party authentication provider metadata by its unique identifier.
        /// </summary>
        /// <param name="partyAuthenticationProviderMetadataId">The unique identifier of the party authentication provider metadata.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>The party authentication provider metadata if found; otherwise, a NotFound or error response.</returns>
        [HttpGet("{partyAuthenticationProviderMetadataId}")]
        public async Task<IActionResult> Get(Guid partyAuthenticationProviderMetadataId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get party authentication provider metadata with ID {PartyAuthenticationProviderMetadataId}", partyAuthenticationProviderMetadataId);

            OperationResult<PartyAuthenticationProviderMetadata> result = await _partyAuthenticationProviderMetadataRepository.GetByIdAsync(partyAuthenticationProviderMetadataId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Party authentication provider metadata not found with ID {partyAuthenticationProviderMetadataId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all party authentication provider metadata entities.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>A list of all party authentication provider metadata entities, or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all party authentication provider metadata");

            OperationResult<IEnumerable<PartyAuthenticationProviderMetadata>> result = await _partyAuthenticationProviderMetadataRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new party authentication provider metadata entity.
        /// </summary>
        /// <param name="partyAuthenticationProviderMetadata">The party authentication provider metadata to create.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>The created party authentication provider metadata, or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartyAuthenticationProviderMetadata partyAuthenticationProviderMetadata, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a party authentication provider metadata with ID {PartyAuthenticationProviderMetadataId}", partyAuthenticationProviderMetadata.Id);

            Helpers.Auditing.SetCreatedInfo(partyAuthenticationProviderMetadata, ControllerContext);

            OperationResult<PartyAuthenticationProviderMetadata> result = await _partyAuthenticationProviderMetadataRepository.AddAsync(partyAuthenticationProviderMetadata, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { partyAuthenticationProviderMetadataId = partyAuthenticationProviderMetadata.Id }, partyAuthenticationProviderMetadata)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing party authentication provider metadata entity.
        /// </summary>
        /// <param name="partyAuthenticationProviderMetadata">The party authentication provider metadata to update.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>The updated party authentication provider metadata, or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PartyAuthenticationProviderMetadata partyAuthenticationProviderMetadata, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a party authentication provider metadata with ID {PartyAuthenticationProviderMetadataId}", partyAuthenticationProviderMetadata.Id);

            Helpers.Auditing.SetModifiedInfo(partyAuthenticationProviderMetadata, ControllerContext);

            OperationResult<PartyAuthenticationProviderMetadata> result = await _partyAuthenticationProviderMetadataRepository.UpdateAsync(partyAuthenticationProviderMetadata, cancellationToken);

            return result.Success
                ? Ok(partyAuthenticationProviderMetadata)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a party authentication provider metadata entity by its unique identifier.
        /// </summary>
        /// <param name="partyAuthenticationProviderMetadataId">The unique identifier of the party authentication provider metadata to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>No content if deleted successfully; otherwise, a NotFound or error response.</returns>
        [HttpDelete("{partyAuthenticationProviderMetadataId}")]
        public async Task<IActionResult> Delete(Guid partyAuthenticationProviderMetadataId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete party authentication provider metadata with ID {PartyAuthenticationProviderMetadataId}", partyAuthenticationProviderMetadataId);

            OperationResult<PartyAuthenticationProviderMetadata> result = await _partyAuthenticationProviderMetadataRepository.DeleteAsync(partyAuthenticationProviderMetadataId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Party authentication provider metadata not found with ID {partyAuthenticationProviderMetadataId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
