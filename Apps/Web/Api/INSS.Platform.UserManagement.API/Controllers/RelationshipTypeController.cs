using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing relationship types.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RelationshipTypeController : ControllerBase
    {
        private readonly ILogger<RelationshipTypeController> _logger;
        private readonly IRelationshipTypeRepository _relationshipTypeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipTypeController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="relationshipTypeRepository">The relationship type repository.</param>
        public RelationshipTypeController(ILogger<RelationshipTypeController> logger, IRelationshipTypeRepository relationshipTypeRepository)
        {
            _logger = logger;
            _relationshipTypeRepository = relationshipTypeRepository;
        }

        /// <summary>
        /// Gets a relationship type by its unique identifier.
        /// </summary>
        /// <param name="relationshipTypeId">The unique identifier of the relationship type.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The relationship type if found; otherwise, a not found or error response.</returns>
        [HttpGet("{relationshipTypeId}")]
        public async Task<IActionResult> Get(Guid relationshipTypeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get relationship type with ID {RelationshipTypeId}", relationshipTypeId);

            OperationResult<RelationshipType> result = await _relationshipTypeRepository.GetByIdAsync(relationshipTypeId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Relationship type not found with ID {relationshipTypeId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all relationship types.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all relationship types or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all relationship types");

            OperationResult<IEnumerable<RelationshipType>> result = await _relationshipTypeRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new relationship type.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created relationship type or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RelationshipType relationshipType, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a relationship type with ID {RelationshipTypeId}", relationshipType.Id);

            Helpers.Auditing.SetCreatedInfo(relationshipType, ControllerContext);

            OperationResult<RelationshipType> result = await _relationshipTypeRepository.AddAsync(relationshipType, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { relationshipTypeId = relationshipType.Id }, relationshipType)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing relationship type.
        /// </summary>
        /// <param name="relationshipType">The relationship type to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated relationship type or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RelationshipType relationshipType, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a relationship type with ID {RelationshipTypeId}", relationshipType.Id);

            Helpers.Auditing.SetModifiedInfo(relationshipType, ControllerContext);

            OperationResult<RelationshipType> result = await _relationshipTypeRepository.UpdateAsync(relationshipType, cancellationToken);

            return result.Success
                ? Ok(relationshipType)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a relationship type by its unique identifier.
        /// </summary>
        /// <param name="relationshipTypeId">The unique identifier of the relationship type to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
        [HttpDelete("{relationshipTypeId}")]
        public async Task<IActionResult> Delete(Guid relationshipTypeId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete relationship type with ID {RelationshipTypeId}", relationshipTypeId);

            OperationResult<RelationshipType> result = await _relationshipTypeRepository.DeleteAsync(relationshipTypeId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Relationship type not found with ID {relationshipTypeId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
