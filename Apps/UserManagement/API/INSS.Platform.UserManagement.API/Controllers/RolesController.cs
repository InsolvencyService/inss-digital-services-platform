using INSS.Platform.UserManagement.Core.Entities;
using INSS.Platform.UserManagement.Repository;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing <see cref="Role"/> entities.
    /// Provides endpoints for CRUD operations on roles.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly ILogger<RolesController> _logger;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="roleRepository">The role repository for data access.</param>
        public RolesController(ILogger<RolesController> logger, IRoleRepository roleRepository)
        {
            _logger = logger;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Retrieves a role by their unique identifier.
        /// </summary>
        /// <param name="roleId">The unique identifier of the role.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the role if found; otherwise, <see cref="NotFoundResult"/>.
        /// </returns>
        [HttpGet("{roleId}")]
        public async Task<IActionResult> Get(Guid roleId)
        {
            _logger.LogInformation("Get role with ID {RoleId}", roleId);
            Role? role = await _roleRepository.GetRoleByIdAsync(roleId).ConfigureAwait(false);

            if (role == null)
            {
                _logger.LogWarning("Role not found with ID {RoleId}", roleId);
                return NotFound("Role not found");
            }

            _logger.LogInformation("Role found with ID {RoleId}", roleId);
            return Ok(role);
        }

        /// <summary>
        /// Creates a new role in the data store.
        /// </summary>
        /// <param name="role">The <see cref="Role"/> to create.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedAtActionResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            _logger.LogInformation("Create a role with ID {RoleId}", role.Id);

            Helpers.Auditing.SetCreatedInfo(role, ControllerContext);

            bool result = await _roleRepository.AddRoleAsync(role).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to create role {Name}", role.Name);
                return StatusCode(500, "Failed to create role.");
            }

            _logger.LogInformation("Role created with ID {RoleId} {Name}", role.Id, role.Name);
            return CreatedAtAction(nameof(Get), new { roleId = role.Id }, role);
        }

        /// <summary>
        /// Updates an existing role in the data store.
        /// </summary>
        /// <param name="role">The <see cref="Role"/> to update.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Role role)
        {
            _logger.LogInformation("Update a role with ID {RoleId}", role.Id);

            Helpers.Auditing.SetModifiedInfo(role, ControllerContext);

            bool result = await _roleRepository.UpdateRoleAsync(role).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to update role with ID {RoleId}", role.Id);
                return StatusCode(500, "Failed to update role.");
            }

            _logger.LogInformation("Role updated with ID {RoleId} {Name}", role.Id, role.Name);
            return Ok(role);
        }

        /// <summary>
        /// Deletes a role from the data store by their unique identifier.
        /// </summary>
        /// <param name="roleId">The unique identifier of the role to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; otherwise, <see cref="NotFoundResult"/> or <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{roleId}")]
        public async Task<IActionResult> Delete(Guid roleId)
        {
            //TODO: Check for related entities before allowing delete
            // Should we just mark as inactive instead e.g. IsDeleted?
            _logger.LogInformation("Delete role with ID {RoleId}", roleId);

            Role? role = await _roleRepository.GetRoleByIdAsync(roleId).ConfigureAwait(false);
            if (role == null)
            {
                _logger.LogWarning("Role not found with ID {RoleId}", roleId);
                return NotFound("Role not found");
            }

            bool result = await _roleRepository.DeleteRoleAsync(role).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError("Failed to delete role with ID {RoleId}", roleId);
                return StatusCode(500, "Failed to delete role.");
            }

            _logger.LogInformation("Role deleted with ID {RoleId}", roleId);
            return NoContent();
        }
    }
}
