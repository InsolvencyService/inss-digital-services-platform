using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing Individual entities.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class IndividualController : ControllerBase
    {
        private readonly ILogger<IndividualController> _logger;
        private readonly IIndividualRepository _individualRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndividualController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for logging operations.</param>
        /// <param name="individualRepository">Repository for individual data access.</param>
        public IndividualController(ILogger<IndividualController> logger, IIndividualRepository individualRepository)
        {
            _logger = logger;
            _individualRepository = individualRepository;
        }

        /// <summary>
        /// Gets an individual by their unique identifier.
        /// </summary>
        /// <param name="individualId">The unique identifier of the individual.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="OkObjectResult"/> with the individual if found,
        /// <see cref="NotFoundObjectResult"/> if not found,
        /// or <see cref="StatusCodeResult"/> for other errors.
        /// </returns>
        [HttpGet("{individualId}")]
        public async Task<IActionResult> Get(Guid individualId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get individual with ID {IndividualId}", individualId);

            OperationResult<Individual> result = await _individualRepository.GetByIdAsync(individualId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Individual not found with ID {individualId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all individuals.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="OkObjectResult"/> with the list of individuals if successful,
        /// or <see cref="StatusCodeResult"/> for errors.
        /// </returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all individuals");

            OperationResult<IEnumerable<Individual>> result = await _individualRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new individual.
        /// </summary>
        /// <param name="individual">The individual entity to create.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="CreatedAtActionResult"/> if creation is successful,
        /// or <see cref="StatusCodeResult"/> for errors.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Individual individual, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create an individual with ID {IndividualId}", individual.Id);

            Helpers.Auditing.SetCreatedInfo(individual, ControllerContext);

            OperationResult<Individual> result = await _individualRepository.AddAsync(individual, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { individualId = individual.Id }, individual)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing individual.
        /// </summary>
        /// <param name="individual">The individual entity with updated information.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="OkObjectResult"/> if update is successful,
        /// or <see cref="StatusCodeResult"/> for errors.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Individual individual, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update an individual with ID {IndividualId}", individual.Id);

            Helpers.Auditing.SetModifiedInfo(individual, ControllerContext);

            OperationResult<Individual> result = await _individualRepository.UpdateAsync(individual, cancellationToken);

            return result.Success
                ? Ok(individual)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes an individual by their unique identifier.
        /// </summary>
        /// <param name="individualId">The unique identifier of the individual to delete.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="NoContentResult"/> if deletion is successful,
        /// <see cref="NotFoundObjectResult"/> if not found,
        /// or <see cref="StatusCodeResult"/> for other errors.
        /// </returns>
        [HttpDelete("{individualId}")]
        public async Task<IActionResult> Delete(Guid individualId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete individual with ID {IndividualId}", individualId);

            OperationResult<Individual> result = await _individualRepository.DeleteAsync(individualId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Individual not found with ID {individualId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
