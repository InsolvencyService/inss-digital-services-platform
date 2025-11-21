using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing resource permissions.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ResourcePermissionController : ControllerBase
    {
        private readonly ILogger<ResourcePermissionController> _logger;
        private readonly IResourcePermissionRepository _resourcePermissionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePermissionController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="resourcePermissionRepository">The resource permission repository.</param>
        public ResourcePermissionController(ILogger<ResourcePermissionController> logger, IResourcePermissionRepository resourcePermissionRepository)
        {
            _logger = logger;
            _resourcePermissionRepository = resourcePermissionRepository;
        }

        /// <summary>
        /// Gets a resource permission by its unique identifier.
        /// </summary>
        /// <param name="resourcePermissionId">The unique identifier of the resource permission.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The resource permission if found; otherwise, a not found or error response.</returns>
        [HttpGet("{resourcePermissionId}")]
        public async Task<IActionResult> Get(Guid resourcePermissionId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get resource permission with ID {ResourceId}", resourcePermissionId);

            OperationResult<ResourcePermission> result = await _resourcePermissionRepository.GetByIdAsync(resourcePermissionId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Resource permission not found with ID {resourcePermissionId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all resource permissions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all resource permissions or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all resource permissions");

            OperationResult<IEnumerable<ResourcePermission>> result = await _resourcePermissionRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new resource permission.
        /// </summary>
        /// <param name="resourcePermission">The resource permission to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created resource permission or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ResourcePermission resourcePermission, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a resource permission with ID {ResourcePermissionId}", resourcePermission.Id);

            Helpers.Auditing.SetCreatedInfo(resourcePermission, ControllerContext);

            OperationResult<ResourcePermission> result = await _resourcePermissionRepository.AddAsync(resourcePermission, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { resourcePermissionId = resourcePermission.Id }, resourcePermission)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing resource permission.
        /// </summary>
        /// <param name="resourcePermission">The resource permission to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated resource permission or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ResourcePermission resourcePermission, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a resource permission with ID {ResourcePermissionId}", resourcePermission.Id);

            Helpers.Auditing.SetModifiedInfo(resourcePermission, ControllerContext);

            OperationResult<ResourcePermission> result = await _resourcePermissionRepository.UpdateAsync(resourcePermission, cancellationToken);

            return result.Success
                ? Ok(resourcePermission)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a resource permission by its unique identifier.
        /// </summary>
        /// <param name="resourcePermissionId">The unique identifier of the resource permission to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
        [HttpDelete("{resourcePermissionId}")]
        public async Task<IActionResult> Delete(Guid resourcePermissionId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete resource permission with ID {ResourcePermissionId}", resourcePermissionId);

            OperationResult<ResourcePermission> result = await _resourcePermissionRepository.DeleteAsync(resourcePermissionId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Resource permission not found with ID {resourcePermissionId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
