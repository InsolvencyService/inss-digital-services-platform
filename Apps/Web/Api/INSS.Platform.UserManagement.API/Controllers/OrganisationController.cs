using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing Organisation entities.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly IOrganisationRepository _organisationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging operations.</param>
        /// <param name="organisationRepository">Repository for organisation data access.</param>
        public OrganisationController(ILogger<OrganisationController> logger, IOrganisationRepository organisationRepository)
        {
            _logger = logger;
            _organisationRepository = organisationRepository;
        }

        /// <summary>
        /// Gets an organisation by its unique identifier.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="OkObjectResult"/> with the organisation if found,
        /// <see cref="NotFoundObjectResult"/> if not found,
        /// or <see cref="StatusCodeResult"/> for other errors.
        /// </returns>
        [HttpGet("{organisationId}")]
        public async Task<IActionResult> Get(Guid organisationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get organisation with ID {OrganisationId}", organisationId);

            OperationResult<Organisation> result = await _organisationRepository.GetByIdAsync(organisationId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Organisation not found with ID {organisationId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all organisations.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="OkObjectResult"/> with the list of organisations if successful,
        /// or <see cref="StatusCodeResult"/> for errors.
        /// </returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all organisations");

            OperationResult<IEnumerable<Organisation>> result = await _organisationRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new organisation.
        /// </summary>
        /// <param name="organisation">The organisation entity to create.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="CreatedAtActionResult"/> with the created organisation if successful,
        /// or <see cref="StatusCodeResult"/> for errors.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Organisation organisation, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create an organisation with ID {OrganisationId}", organisation.Id);

            Helpers.Auditing.SetCreatedInfo(organisation, ControllerContext);

            OperationResult<Organisation> result = await _organisationRepository.AddAsync(organisation, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { organisationId = organisation.Id }, organisation)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing organisation.
        /// </summary>
        /// <param name="organisation">The organisation entity with updated data.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="OkObjectResult"/> with the updated organisation if successful,
        /// or <see cref="StatusCodeResult"/> for errors.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Organisation organisation, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update an organisation with ID {OrganisationId}", organisation.Id);

            Helpers.Auditing.SetModifiedInfo(organisation, ControllerContext);

            OperationResult<Organisation> result = await _organisationRepository.UpdateAsync(organisation, cancellationToken);

            return result.Success
                ? Ok(organisation)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes an organisation by its unique identifier.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="NoContentResult"/> if deletion is successful,
        /// <see cref="NotFoundObjectResult"/> if not found,
        /// or <see cref="StatusCodeResult"/> for other errors.
        /// </returns>
        [HttpDelete("{organisationId}")]
        public async Task<IActionResult> Delete(Guid organisationId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete organisation with ID {OrganisationId}", organisationId);

            OperationResult<Organisation> result = await _organisationRepository.DeleteAsync(organisationId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Organisation not found with ID {organisationId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
