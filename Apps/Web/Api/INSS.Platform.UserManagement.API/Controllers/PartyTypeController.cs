using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing party types.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class PartyTypeController : ControllerBase
    {
        private readonly ILogger<PartyTypeController> _logger;
        private readonly IPartyTypeRepository _partyTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyTypeController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="partyTypeRepository">The party type repository.</param>
        public PartyTypeController(ILogger<PartyTypeController> logger, IPartyTypeRepository partyTypeRepository)
        {
            _logger = logger;
            _partyTypeRepository = partyTypeRepository;
        }

        /// <summary>
        /// Gets a party type by its unique identifier.
        /// </summary>
        /// <param name="partyTypeId">The unique identifier of the party type.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The party type if found; otherwise, a not found or error response.</returns>
        [HttpGet("{partyTypeId}")]
        public async Task<IActionResult> Get(Guid partyTypeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get party type with ID {PartyTypeId}", partyTypeId);

            OperationResult<PartyType> result = await _partyTypeRepository.GetByIdAsync(partyTypeId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Party type not found with ID {partyTypeId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all party types.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all party types or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all party types");

            OperationResult<IEnumerable<PartyType>> result = await _partyTypeRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new party type.
        /// </summary>
        /// <param name="partyType">The party type to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created party type or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartyType partyType, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a party type with ID {PartyTypeId}", partyType.Id);

            Helpers.Auditing.SetCreatedInfo(partyType, ControllerContext);

            OperationResult<PartyType> result = await _partyTypeRepository.AddAsync(partyType, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { partyTypeId = partyType.Id }, partyType)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing party type.
        /// </summary>
        /// <param name="partyType">The party type to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated party type or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PartyType partyType, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a party type with ID {PartyTypeId}", partyType.Id);

            Helpers.Auditing.SetModifiedInfo(partyType, ControllerContext);

            OperationResult<PartyType> result = await _partyTypeRepository.UpdateAsync(partyType, cancellationToken);

            return result.Success
                ? Ok(partyType)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a party type by its unique identifier.
        /// </summary>
        /// <param name="partyTypeId">The unique identifier of the party type to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
        [HttpDelete("{partyTypeId}")]
        public async Task<IActionResult> Delete(Guid partyTypeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete party type with ID {PartyTypeId}", partyTypeId);

            OperationResult<PartyType> result = await _partyTypeRepository.DeleteAsync(partyTypeId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Party type not found with ID {partyTypeId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
