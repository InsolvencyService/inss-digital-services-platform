using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing roles.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="roleRepository">The role repository instance.</param>
        public RoleController(ILogger<RoleController> logger, IRoleRepository roleRepository)
        {
            _logger = logger;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Gets a role by its unique identifier.
        /// </summary>
        /// <param name="roleId">The unique identifier of the role.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The role if found; otherwise, a not found or error response.</returns>
        [HttpGet("{roleId}")]
        public async Task<IActionResult> Get(Guid roleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get role with ID {RoleId}", roleId);

            OperationResult<Role> result = await _roleRepository.GetByIdAsync(roleId, cancellationToken);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Role not found with ID {roleId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of all roles or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all roles");

            OperationResult<IEnumerable<Role>> result = await _roleRepository.GetAsync(cancellationToken);

            return result.Success 
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new role.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created role or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create a role with ID {RoleId}", role.Id);

            Helpers.Auditing.SetCreatedInfo(role, ControllerContext);

            OperationResult<Role> result = await _roleRepository.AddAsync(role, cancellationToken);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { roleId = role.Id }, role)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated role or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Role role, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update a role with ID {RoleId}", role.Id);

            Helpers.Auditing.SetModifiedInfo(role, ControllerContext);

            OperationResult<Role> result = await _roleRepository.UpdateAsync(role, cancellationToken);

            return result.Success
                ? Ok(role)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes a role by its unique identifier.
        /// </summary>
        /// <param name="roleId">The unique identifier of the role to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if successful; otherwise, a not found or error response.</returns>
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(Guid roleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete role with ID {RoleId}", roleId);

            OperationResult<Role> result = await _roleRepository.DeleteAsync(roleId, cancellationToken);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Role not found with ID {roleId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
