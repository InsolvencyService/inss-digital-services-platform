using INSS.Platform.UserManagement.Core.Entities;
using INSS.Platform.UserManagement.Repository;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing <see cref="Organisation"/> entities.
    /// Provides endpoints for CRUD operations on organisations.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class OrganisationsController : ControllerBase
    {
        private readonly ILogger<OrganisationsController> _logger;
        private readonly IOrganisationRepository _organisationRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrganisationsController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="organisationRepository">The organisation repository for data access.</param>
        /// <param name="applicationRepository">The application repository for data access.</param>
        /// <param name="userRepository">The user repository for data access.</param>
        /// <param name="roleRepository">The role repository for data access.</param>
        public OrganisationsController(
            ILogger<OrganisationsController> logger,
            IOrganisationRepository organisationRepository,
            IUserRepository userRepository,
            IApplicationRepository applicationRepository,
            IRoleRepository roleRepository)
        {
            _logger = logger;
            _organisationRepository = organisationRepository;
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Retrieves an organisation by their unique identifier.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the organisation if found; otherwise, <see cref="NotFoundResult"/>.
        /// </returns>
        [HttpGet("{organisationId}")]
        public async Task<IActionResult> Get(Guid organisationId)
        {
            _logger.LogInformation("Get organisation with ID {OrganisationId}", organisationId);
            Organisation? organisation = await _organisationRepository.GetOrganisationByIdAsync(organisationId).ConfigureAwait(false);

            if (organisation == null)
            {
                _logger.LogWarning("Organisation not found with ID {OrganisationId}", organisationId);
                return NotFound("Organisation not found");
            }

            _logger.LogInformation("Organisation found with ID {OrganisationId}", organisationId);
            return Ok(organisation);
        }

        /// <summary>
        /// Retrieves all organisations.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="Organisation"/> entities.
        /// Returns <see cref="OkObjectResult"/> with the organisations if found; otherwise, an empty collection.
        /// </returns>
        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Get all organisations");
            IEnumerable<Organisation> organisations = await _organisationRepository.GetOrganisationsAsync().ConfigureAwait(false);

            _logger.LogInformation("{OrganisationCount} Organisations found", organisations.Count());
            return Ok(organisations);
        }

        /// <summary>
        /// Retrieves all users associated with a specific organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="User"/> entities belonging to the specified organisation.
        /// Returns <see cref="OkObjectResult"/> with the users if found; otherwise, an empty collection.
        /// </returns>
        [HttpGet("{organisationId}/users")]
        public async Task<IActionResult> GetUsers(Guid organisationId)
        {
            _logger.LogInformation("Get organisation users for organisation with ID {OrganisationID}", organisationId);
            IEnumerable<User> users = await _userRepository.GetUsersByOrganisationAsync(organisationId).ConfigureAwait(false);

            _logger.LogInformation("{UserCount} Users found for organisation with ID {OrganisationID}", users.Count(), organisationId);
            return Ok(users);
        }

        /// <summary>
        /// Retrieves all applications associated with a specific organisation user.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="Application"/> entities
        /// linked to the specified organisation and user.
        /// Returns <see cref="OkObjectResult"/> with the applications if found; otherwise, an empty collection.
        /// </returns>
        [HttpGet("{organisationId}/users/{userId}/applications")]
        public async Task<IActionResult> GetApplicationsByOrganisationUser(Guid organisationId, Guid userId)
        {
            _logger.LogInformation("Get applications by organisation with ID {OrganisationId} and user with ID {UserId}", organisationId, userId);
            IEnumerable<Application> applications = await _applicationRepository.GetApplicationsByOrganisationUserAsync(organisationId, userId);

            _logger.LogInformation("{ApplicationCount} Applications found for organisation with ID {OrganisationId} and user with ID {UserId}", applications.Count(), organisationId, userId);
            return Ok(applications);
        }

        /// <summary>
        /// Retrieves all roles associated with a specific organisation user and application.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="Role"/> entities
        /// linked to the specified organisation, user, and application.
        /// Returns <see cref="OkObjectResult"/> with the roles if found; otherwise, an empty collection.
        /// </returns>
        [HttpGet("{organisationId}/users/{userId}/applications/{applicationId}/roles")]
        public async Task<IActionResult> GetRolesByOrganisationUserApplication(Guid organisationId, Guid userId, Guid applicationId)
        {
            _logger.LogInformation("Get roles by organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", organisationId, userId, applicationId);
            IEnumerable<Role> roles = await _roleRepository.GetRolesByOrganisationUserApplicationAsync(organisationId, userId, applicationId);

            _logger.LogInformation("{RoleCount} Roles found for organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", roles.Count(), organisationId, userId, applicationId);
            return Ok(roles);
        }

        /// <summary>
        /// Creates a new organisation in the data store.
        /// </summary>
        /// <param name="organisation">The <see cref="Organisation"/> to create.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedAtActionResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Organisation organisation)
        {
            _logger.LogInformation("Create an organisation with ID {OrganisationId}", organisation.Id);

            Helpers.Auditing.SetCreatedInfo(organisation, ControllerContext);

            bool result = await _organisationRepository.AddOrganisationAsync(organisation).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to create organisation {Name}", organisation.Name);
                return StatusCode(500, "Failed to create organisation.");
            }

            _logger.LogInformation("Organisation created with ID {OrganisationId} {Name}", organisation.Id, organisation.Name);
            return CreatedAtAction(nameof(Get), new { organisationId = organisation.Id }, organisation);
        }

        /// <summary>
        /// Updates an existing organisation in the data store.
        /// </summary>
        /// <param name="organisation">The <see cref="Organisation"/> to update.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Organisation organisation)
        {
            _logger.LogInformation("Update an organisation with ID {OrganisationId}", organisation.Id);

            Helpers.Auditing.SetModifiedInfo(organisation, ControllerContext);

            bool result = await _organisationRepository.UpdateOrganisationAsync(organisation).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to update organisation with ID {OrganisationId}", organisation.Id);
                return StatusCode(500, "Failed to update organisation.");
            }

            _logger.LogInformation("Organisation updated with ID {OrganisationId} {Name}", organisation.Id, organisation.Name);
            return Ok(organisation);
        }

        /// <summary>
        /// Deletes an organisation from the data store by their unique identifier.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; otherwise, <see cref="NotFoundResult"/> or <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{organisationId}")]
        public async Task<IActionResult> Delete(Guid organisationId)
        {
            //TODO: Check for related entities before allowing delete
            // Should we just mark as inactive instead e.g. IsDeleted?
            _logger.LogInformation("Delete organisation with ID {OrganisationId}", organisationId);

            Organisation? organisation = await _organisationRepository.GetOrganisationByIdAsync(organisationId).ConfigureAwait(false);
            if (organisation == null)
            {
                _logger.LogWarning("Organisation not found with ID {OrganisationId}", organisationId);
                return NotFound("Organisation not found");
            }

            bool result = await _organisationRepository.DeleteOrganisationAsync(organisation).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError("Failed to delete organisation with ID {OrganisationId}", organisationId);
                return StatusCode(500, "Failed to delete organisation.");
            }

            _logger.LogInformation("Organisation deleted with ID {OrganisationId}", organisationId);
            return NoContent();
        }

        /// <summary>
        /// Adds a user to an organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user to add.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedResult"/> if successful; <see cref="ConflictResult"/> if the user already exists in the organisation; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost("{organisationId}/users/{userId}")]
        public async Task<IActionResult> AddUser(Guid organisationId, Guid userId)
        {
            _logger.LogInformation("Add user with ID {UserId} into organisation with ID {OrganisationId}", userId, organisationId);

            bool organisationUserExists = await _organisationRepository.OrganisationUserExistsAsync(organisationId, userId).ConfigureAwait(false);
            if (organisationUserExists)
            {
                _logger.LogWarning("User with ID {UserID} already exists in organisation with ID {OrganisationId}", userId, organisationId);
                return Conflict("User already exists in organisation.");
            }

            OrganisationUser organisationUser = new()
            {
                OrganisationId = organisationId,
                UserId = userId
            };

            Helpers.Auditing.SetCreatedInfo(organisationUser, ControllerContext);

            bool result = await _organisationRepository.AddOrganisationUserAsync(organisationUser).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to add user with ID {UserID} to organisation with ID {OrganisationId}", userId, organisationId);
                return StatusCode(500, "Failed to add user to organisation.");
            }

            _logger.LogInformation("user with ID {UserID} added to organisation with ID {OrganisationId}", userId, organisationId);
            return Created();
        }

        /// <summary>
        /// Removes a user from an organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user to remove.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; <see cref="NotFoundResult"/> if the user is not found in the organisation; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{organisationId}/users/{userId}")]
        public async Task<IActionResult> RemoveUser(Guid organisationId, Guid userId)
        {
            _logger.LogInformation("Remove user with ID {UserId} from organisation with ID {OrganisationId}", userId, organisationId);

            bool organisationUserExists = await _organisationRepository.OrganisationUserExistsAsync(organisationId, userId).ConfigureAwait(false);
            if (!organisationUserExists)
            {
                _logger.LogWarning("User with ID {UserID} does not exist in organisation with ID {OrganisationId}", userId, organisationId);
                return NotFound("User not found in organisation");
            }

            bool result = await _organisationRepository.RemoveOrganisationUserAsync(organisationId, userId).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to remove user with ID {UserID} from organisation with ID {OrganisationId}", userId, organisationId);
                return StatusCode(500, "Failed to remove user from organisation.");
            }

            _logger.LogInformation("User with ID {UserID} removed from organisation with ID {OrganisationId}", userId, organisationId);
            return NoContent();
        }

        /// <summary>
        /// Removes all users from an organisation.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{organisationId}/users")]
        public async Task<IActionResult> RemoveUsers(Guid organisationId)
        {
            _logger.LogInformation("Remove all user from organisation with ID {OrganisationId}", organisationId);

            bool result = await _organisationRepository.RemoveAllOrganisationUsersAsync(organisationId).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to remove all users from organisation with ID {OrganisationId}", organisationId);
                return StatusCode(500, "Failed to remove all users from organisation.");
            }

            _logger.LogInformation("Removed all users from organisation with ID {OrganisationId}", organisationId);
            return NoContent();
        }

        /// <summary>
        /// Adds an application role to a specific organisation user.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <param name="roleId">The unique identifier of the role.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedResult"/> if successful; <see cref="ConflictResult"/> if the role is already assigned; <see cref="NotFoundResult"/> if the organisation user or application role is not found; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost("{organisationId}/users/{userId}/applications/{applicationId}/roles/{roleId}")]
        public async Task<IActionResult> AddApplicationRoleToOrganisationUser(Guid organisationId, Guid userId, Guid applicationId, Guid roleId)
        {
            _logger.LogInformation("Add application role for application with ID {ApplicationId} and role with ID {RoleId} to organisation with ID {OrganisationId} and user with ID {UserId}", applicationId, roleId, organisationId, userId);
            bool organisationUserExists = await _organisationRepository.OrganisationUserExistsAsync(organisationId, userId).ConfigureAwait(false);
            if (!organisationUserExists)
            {
                _logger.LogWarning("Organisation user not found for organisation with ID {OrganisationId} and user with ID {UserId}", organisationId, userId);
                return NotFound("Organisation user not found");
            }

            bool applicationRoleExists = await _applicationRepository.ApplicationRoleExistsAsync(applicationId, roleId).ConfigureAwait(false);
            if (!applicationRoleExists)
            {
                _logger.LogWarning("Application role not found for application with ID {ApplicationId} and role with ID {RoleId}", applicationId, roleId);
                return NotFound("Application user not found");
            }

            OrganisationUser? organisationUser = await _organisationRepository.GetOrganisationUserAsync(organisationId, userId).ConfigureAwait(false);
            ApplicationRole? applicationRole = await _applicationRepository.GetApplicationRoleAsync(applicationId, roleId).ConfigureAwait(false);

            bool associationExists = await _organisationRepository.OrganisationUserApplicationRoleExistsAsync(organisationUser!.Id, applicationRole!.Id).ConfigureAwait(false);
            if (associationExists)
            {
                _logger.LogWarning("Role with ID {RoleId} already assigned to organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", roleId, organisationId, userId, applicationId);
                return Conflict("Role is already assigned to organisation user for the application.");
            }

            OrganisationUserApplicationRole organisationUserApplicationRole = new()
            {
                OrganisationUserId = organisationUser.Id,
                ApplicationRoleId = applicationRole.Id,
            };

            Helpers.Auditing.SetCreatedInfo(organisationUserApplicationRole, ControllerContext);

            bool result = await _organisationRepository.AddOrganisationUserApplicationRoleAsync(organisationUserApplicationRole).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError("Failed to assign role with ID {RoleId} to organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", roleId, organisationId, userId, applicationId);
                return StatusCode(500, "Failed to assign application role to organisation user");
            }

            _logger.LogInformation("Role with ID {RoleId} assigned to organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", roleId, organisationId, userId, applicationId);
            return Created();
        }

        /// <summary>
        /// Removes a previously assigned application role from a specific organisation user.
        /// </summary>
        /// <param name="organisationId">The unique identifier of the organisation.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="applicationId">The unique identifier of the application.</param>
        /// <param name="roleId">The unique identifier of the role.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; <see cref="NotFoundResult"/> if the role is not assigned to the user or if the organisation user or application role is not found; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{organisationId}/users/{userId}/applications/{applicationId}/roles/{roleId}")]
        public async Task<IActionResult> RemoveApplicationRoleFromOrganisationUser(Guid organisationId, Guid userId, Guid applicationId, Guid roleId)
        {
            _logger.LogInformation("Remove application role for application with ID {ApplicationId} and role with ID {RoleId} from organisation with ID {OrganisationId} and user with ID {UserId}", applicationId, roleId, organisationId, userId);

            bool organisationUserExists = await _organisationRepository.OrganisationUserExistsAsync(organisationId, userId).ConfigureAwait(false);
            if (!organisationUserExists)
            {
                _logger.LogWarning("Organisation user not found for organisation with ID {OrganisationId} and user with ID {UserId}", organisationId, userId);
                return NotFound("Organisation user not found");
            }

            bool applicationRoleExists = await _applicationRepository.ApplicationRoleExistsAsync(applicationId, roleId).ConfigureAwait(false);
            if (!applicationRoleExists)
            {
                _logger.LogWarning("Application role not found for application with ID {ApplicationId} and role with ID {RoleId}", applicationId, roleId);
                return NotFound("Application user not found");
            }

            OrganisationUser? organisationUser = await _organisationRepository.GetOrganisationUserAsync(organisationId, userId).ConfigureAwait(false);
            ApplicationRole? applicationRole = await _applicationRepository.GetApplicationRoleAsync(applicationId, roleId).ConfigureAwait(false);

            bool associationExists = await _organisationRepository.OrganisationUserApplicationRoleExistsAsync(organisationUser!.Id, applicationRole!.Id).ConfigureAwait(false);
            if (!associationExists)
            {
                _logger.LogWarning("Role with ID {RoleId} is not assigned to organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", roleId, organisationId, userId, applicationId);
                return NotFound("Role is not assigned to organisation user for the application.");
            }

            bool result = await _organisationRepository.RemoveOrganisationUserApplicationRoleAsync(organisationUser!.Id, applicationRole!.Id).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError("Failed to remove role with ID {RoleId} from organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", roleId, organisationId, userId, applicationId);
                return StatusCode(500, "Failed to remove application role from organisation user");
            }

            _logger.LogInformation("Role with ID {RoleId} removed from organisation with ID {OrganisationId}, user with ID {UserId} and application with ID {ApplicationId}", roleId, organisationId, userId, applicationId);
            return NoContent();
        }
    }
}