using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing party product roles.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class PartyProductRoleController : ControllerBase
    {
        private readonly ILogger<PartyProductRoleController> _logger;
        private readonly IPartyProductRoleRepository _partyProductRoleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyProductRoleController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="partyProductRoleRepository">The party product role repository.</param>
        public PartyProductRoleController(ILogger<PartyProductRoleController> logger, IPartyProductRoleRepository partyProductRoleRepository)
        {
            _logger = logger;
            _partyProductRoleRepository = partyProductRoleRepository;
        }

        /// <summary>
        /// Gets a party product role by its unique identifier.
        /// </summary>
        /// <param name="partyProductRoleId">The unique identifier of the party product role.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The party product role if found; otherwise, a not found or error response.</returns>
        [HttpGet("{partyProductRoleId}")]
        public async Task<IActionResult> Get(Guid partyProductRoleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get party product role with ID {PartyProductRoleId}", partyProductRoleId);

            OperationResult<PartyProductRole> result = await _partyProductRoleRepository.GetByIdAsync(partyProductRoleId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Party product role not found with ID {partyProductRoleId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all party product roles.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all party product roles or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all party product roles");

            OperationResult<IEnumerable<PartyProductRole>> result = await _partyProductRoleRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new party product role.
        /// </summary>
        /// <param name="partyProductRole">The party product role to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created party product role or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartyProductRole partyProductRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a party product role with ID {PartyProductRoleId}", partyProductRole.Id);

            Helpers.Auditing.SetCreatedInfo(partyProductRole, ControllerContext);

            OperationResult<PartyProductRole> result = await _partyProductRoleRepository.AddAsync(partyProductRole, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { partyProductRoleId = partyProductRole.Id }, partyProductRole)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing party product role.
        /// </summary>
        /// <param name="partyProductRole">The party product role to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated party product role or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PartyProductRole partyProductRole, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a party product role with ID {PartyProductRoleId}", partyProductRole.Id);

            Helpers.Auditing.SetModifiedInfo(partyProductRole, ControllerContext);

            OperationResult<PartyProductRole> result = await _partyProductRoleRepository.UpdateAsync(partyProductRole, cancellationToken);

            return result.Success
                ? Ok(partyProductRole)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a party product role by its unique identifier.
        /// </summary>
        /// <param name="partyProductRoleId">The unique identifier of the party product role to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
        [HttpDelete("{partyProductRoleId}")]
        public async Task<IActionResult> Delete(Guid partyProductRoleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete party product role with ID {PartyProductRoleId}", partyProductRoleId);

            OperationResult<PartyProductRole> result = await _partyProductRoleRepository.DeleteAsync(partyProductRoleId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Party product role not found with ID {partyProductRoleId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
