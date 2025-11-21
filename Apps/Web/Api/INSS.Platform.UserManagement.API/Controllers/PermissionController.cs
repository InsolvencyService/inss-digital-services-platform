using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing permissions.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly ILogger<PermissionController> _logger;
        private readonly IPermissionRepository _permissionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="permissionRepository">The permission repository instance.</param>
        public PermissionController(ILogger<PermissionController> logger, IPermissionRepository permissionRepository)
        {
            _logger = logger;
            _permissionRepository = permissionRepository;
        }

        /// <summary>
        /// Gets a permission by its unique identifier.
        /// </summary>
        /// <param name="permissionId">The unique identifier of the permission.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The permission if found; otherwise, a not found or error response.</returns>
        [HttpGet("{permissionId}")]
        public async Task<IActionResult> Get(Guid permissionId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get permission with ID {PermissionId}", permissionId);

            OperationResult<Permission> result = await _permissionRepository.GetByIdAsync(permissionId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Permission not found with ID {permissionId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all permissions or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all permissions");

            OperationResult<IEnumerable<Permission>> result = await _permissionRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new permission.
        /// </summary>
        /// <param name="permission">The permission to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created permission or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Permission permission, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a permission with ID {PermissionId}", permission.Id);

            Helpers.Auditing.SetCreatedInfo(permission, ControllerContext);

            OperationResult<Permission> result = await _permissionRepository.AddAsync(permission, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { permissionId = permission.Id }, permission)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing permission.
        /// </summary>
        /// <param name="permission">The permission to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated permission or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Permission permission, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a permission with ID {PermissionId}", permission.Id);

            Helpers.Auditing.SetModifiedInfo(permission, ControllerContext);

            OperationResult<Permission> result = await _permissionRepository.UpdateAsync(permission, cancellationToken);

            return result.Success
                ? Ok(permission)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a permission by its unique identifier.
        /// </summary>
        /// <param name="permissionId">The unique identifier of the permission to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
        [HttpDelete("{permissionId}")]
        public async Task<IActionResult> Delete(Guid permissionId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete permission with ID {PermissionId}", permissionId);

            OperationResult<Permission> result = await _permissionRepository.DeleteAsync(permissionId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Permission not found with ID {permissionId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
