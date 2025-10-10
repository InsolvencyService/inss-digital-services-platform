using INSS.Platform.UserManagement.Core.Entities;
using INSS.Platform.UserManagement.Repository;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing <see cref="Application"/> entities.
    /// Provides endpoints for CRUD operations on applications.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class ApplicationsController : ControllerBase
    {
        private readonly ILogger<ApplicationsController> _logger;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="applicationRepository">The application repository for data access.</param>
        /// <param name="roleRepository">The role repository for data access.</param>
        public ApplicationsController(ILogger<ApplicationsController> logger, IApplicationRepository applicationRepository, IRoleRepository roleRepository)
        {
            _logger = logger;
            _applicationRepository = applicationRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Retrieves an application by their unique identifier.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the application if found; otherwise, <see cref="NotFoundResult"/>.
        /// </returns>
        [HttpGet("{applicationId}")]
        public async Task<IActionResult> Get(Guid applicationId)
        {
            _logger.LogInformation("Get application with ID {ApplicationId}", applicationId);
            Application? application = await _applicationRepository.GetApplicationByIdAsync(applicationId).ConfigureAwait(false);

            if (application == null)
            {
                _logger.LogWarning("Application not found with ID {ApplicationId}", applicationId);
                return NotFound("Application not found");
            }

            _logger.LogInformation("Application found with ID {ApplicationId}", applicationId);
            return Ok(application);
        }

        /// <summary>
        /// Retrieves all <see cref="Application"/> entities from the data store.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="Application"/> entities.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetApplicationsAsync()
        {
            _logger.LogInformation("Get all applications");
            IEnumerable<Application> applications = await _applicationRepository.GetApplicationsAsync().ConfigureAwait(false);

            _logger.LogInformation("{ApplicationCount} Applications found", applications.Count());
            return Ok(applications);
        }

        /// <summary>
        /// Retrieves all roles associated with a specific application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="Role"/> entities if found; otherwise, an empty collection.
        /// </returns>
        [HttpGet("{applicationId}/roles")]
        public async Task<IActionResult> GetRoles(Guid applicationId)
        {
            _logger.LogInformation("Get application roles for application with ID {ApplicationId}", applicationId);
            IEnumerable<Role> roles = await _roleRepository.GetRolesByApplicationAsync(applicationId).ConfigureAwait(false);

            _logger.LogInformation("{RoleCount} Roles found for application with ID {ApplicationID}", roles.Count(), applicationId);
            return Ok(roles);
        }

        /// <summary>
        /// Creates a new application in the data store.
        /// </summary>
        /// <param name="application">The <see cref="Application"/> to create.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedAtActionResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Application application)
        {
            _logger.LogInformation("Create an application with ID {ApplicationId}", application.Id);

            Helpers.Auditing.SetCreatedInfo(application, ControllerContext);

            bool result = await _applicationRepository.AddApplicationAsync(application).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to create application {Name}", application.Name);
                return StatusCode(500, "Failed to create application.");
            }

            _logger.LogInformation("Application created with ID {ApplicationId} {Name}", application.Id, application.Name);
            return CreatedAtAction(nameof(Get), new { applicationId = application.Id }, application);
        }

        /// <summary>
        /// Adds a role to an application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <param name="roleId">The unique identifier of the role to add.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedResult"/> if successful; <see cref="ConflictResult"/> if the role already exists in the application; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost("{applicationId}/roles/{roleId}")]
        public async Task<IActionResult> AddRole(Guid applicationId, Guid roleId)
        {
            _logger.LogInformation("Add role with ID {RoleId} into application with ID {ApplicationId}", roleId, applicationId);

            bool applicationRoleExists = await _applicationRepository.ApplicationRoleExistsAsync(applicationId, roleId).ConfigureAwait(false);
            if (applicationRoleExists)
            {
                _logger.LogWarning("Role with ID {RoleID} already exists in application with ID {ApplicationId}", roleId, applicationId);
                return Conflict("Role already exists in application.");
            }

            ApplicationRole applicationRole = new()
            {
                ApplicationId = applicationId,
                RoleId = roleId
            };

            Helpers.Auditing.SetCreatedInfo(applicationRole, ControllerContext);

            bool result = await _applicationRepository.AddApplicationRoleAsync(applicationRole).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to add role with ID {RoleID} to application with ID {ApplicationId}", roleId, applicationId);
                return StatusCode(500, "Failed to add role to application.");
            }

            _logger.LogInformation("role with ID {RoleID} added to application with ID {ApplicationId}", roleId, applicationId);
            return Created();
        }

        /// <summary>
        /// Removes a role from an application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <param name="roleId">The unique identifier of the role to remove.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; <see cref="NotFoundResult"/> if the role is not found in the application; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{applicationId}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRole(Guid applicationId, Guid roleId)
        {
            _logger.LogInformation("Remove role with ID {RoleId} from application with ID {ApplicationId}", roleId, applicationId);

            bool applicationRoleExists = await _applicationRepository.ApplicationRoleExistsAsync(applicationId, roleId).ConfigureAwait(false);
            if (!applicationRoleExists)
            {
                _logger.LogWarning("Role with ID {RoleID} does not exist in application with ID {ApplicationId}", roleId, applicationId);
                return NotFound("Role not found in application");
            }

            bool result = await _applicationRepository.RemoveApplicationRoleAsync(applicationId, roleId).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to remove role with ID {RoleID} from application with ID {ApplicationId}", roleId, applicationId);
                return StatusCode(500, "Failed to remove role from application.");
            }

            _logger.LogInformation("Role with ID {RoleID} removed from application with ID {ApplicationId}", roleId, applicationId);
            return NoContent();
        }

        /// <summary>
        /// Removes all roles from an application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{applicationId}/roles")]
        public async Task<IActionResult> RemoveRoles(Guid applicationId)
        {
            _logger.LogInformation("Remove all role from application with ID {ApplicationId}", applicationId);

            bool result = await _applicationRepository.RemoveAllApplicationRolesAsync(applicationId).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to remove all roles from application with ID {ApplicationId}", applicationId);
                return StatusCode(500, "Failed to remove all roles from application.");
            }

            _logger.LogInformation("Removed all roles from application with ID {ApplicationId}", applicationId);
            return NoContent();
        }

        /// <summary>
        /// Updates an existing application in the data store.
        /// </summary>
        /// <param name="application">The <see cref="Application"/> to update.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Application application)
        {
            _logger.LogInformation("Update an application with ID {ApplicationId}", application.Id);

            Helpers.Auditing.SetModifiedInfo(application, ControllerContext);

            bool result = await _applicationRepository.UpdateApplicationAsync(application).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to update application with ID {ApplicationId}", application.Id);
                return StatusCode(500, "Failed to update application.");
            }

            _logger.LogInformation("Application updated with ID {ApplicationId} {Name}", application.Id, application.Name);
            return Ok(application);
        }

        /// <summary>
        /// Deletes an application from the data store by their unique identifier.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; otherwise, <see cref="NotFoundResult"/> or <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> Delete(Guid applicationId)
        {
            //TODO: Check for related entities before allowing delete
            // Should we just mark as inactive instead e.g. IsDeleted?
            _logger.LogInformation("Delete application with ID {ApplicationId}", applicationId);

            Application? application = await _applicationRepository.GetApplicationByIdAsync(applicationId).ConfigureAwait(false);
            if (application == null)
            {
                _logger.LogWarning("Application not found with ID {ApplicationId}", applicationId);
                return NotFound("Application not found");
            }

            bool result = await _applicationRepository.DeleteApplicationAsync(application).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError("Failed to delete application with ID {ApplicationId}", applicationId);
                return StatusCode(500, "Failed to delete application.");
            }

            _logger.LogInformation("Application deleted with ID {ApplicationId}", applicationId);
            return NoContent();
        }
    }
}
