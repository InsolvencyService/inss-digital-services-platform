using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing role metadata.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RoleMetadataController : ControllerBase
    {
        private readonly ILogger<RoleMetadataController> _logger;
        private readonly IRoleMetadataRepository _roleMetadataRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleMetadataController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="roleMetadataRepository">The role metadata repository.</param>
        public RoleMetadataController(ILogger<RoleMetadataController> logger, IRoleMetadataRepository roleMetadataRepository)
        {
            _logger = logger;
            _roleMetadataRepository = roleMetadataRepository;
        }

        /// <summary>
        /// Gets a specific role metadata by its unique identifier.
        /// </summary>
        /// <param name="roleMetadataId">The unique identifier of the role metadata.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The role metadata if found; otherwise, a not found or error response.</returns>
        [HttpGet("{roleMetadataId}")]
        public async Task<IActionResult> Get(Guid roleMetadataId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get role metadata with ID {RoleMetadataId}", roleMetadataId);

            OperationResult<RoleMetadata> result = await _roleMetadataRepository.GetByIdAsync(roleMetadataId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Role metadata not found with ID {roleMetadataId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all role metadata.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all role metadata or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all role metadata");

            OperationResult<IEnumerable<RoleMetadata>> result = await _roleMetadataRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new role metadata entry.
        /// </summary>
        /// <param name="roleMetadata">The role metadata to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created role metadata or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoleMetadata roleMetadata, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a role metadata with ID {RoleMetadataId}", roleMetadata.Id);

            Helpers.Auditing.SetCreatedInfo(roleMetadata, ControllerContext);

            OperationResult<RoleMetadata> result = await _roleMetadataRepository.AddAsync(roleMetadata, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { roleMetadataId = roleMetadata.Id }, roleMetadata)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing role metadata entry.
        /// </summary>
        /// <param name="roleMetadata">The role metadata to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated role metadata or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RoleMetadata roleMetadata, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a role metadata with ID {RoleMetadataId}", roleMetadata.Id);

            Helpers.Auditing.SetModifiedInfo(roleMetadata, ControllerContext);

            OperationResult<RoleMetadata> result = await _roleMetadataRepository.UpdateAsync(roleMetadata, cancellationToken);

            return result.Success
                ? Ok(roleMetadata)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a specific role metadata by its unique identifier.
        /// </summary>
        /// <param name="roleMetadataId">The unique identifier of the role metadata to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if successful; otherwise, a not found or error response.</returns>
        [HttpDelete("{roleMetadataId}")]
        public async Task<IActionResult> Delete(Guid roleMetadataId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete role metadata with ID {RoleMetadataId}", roleMetadataId);

            OperationResult<RoleMetadata> result = await _roleMetadataRepository.DeleteAsync(roleMetadataId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Role metadata not found with ID {roleMetadataId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
