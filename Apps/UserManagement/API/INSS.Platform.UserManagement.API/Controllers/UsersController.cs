using INSS.Platform.UserManagement.Core.Entities;
using INSS.Platform.UserManagement.Repository;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// API controller for managing <see cref="User"/> entities.
    /// Provides endpoints for CRUD operations on users.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IOrganisationRepository _organisationRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="userRepository">The user repository for data access.</param>
        /// <param name="organisationRepository">The organisation repository for data access.</param>
        public UsersController(ILogger<UsersController> logger, IUserRepository userRepository, IOrganisationRepository organisationRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _organisationRepository = organisationRepository;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user if found; otherwise, <see cref="NotFoundResult"/>.
        /// </returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(Guid userId)
        {
            _logger.LogInformation("Get user with ID {UserId}", userId);
            User? user = await _userRepository.GetUserByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID {UserId}", userId);
                return NotFound("User not found");
            }

            _logger.LogInformation("User found with ID {UserId}", userId);
            return Ok(user);
        }

        /// <summary>
        /// Retrieves all organisations associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose organisations are to be retrieved.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="Organisation"/> entities linked to the specified user.
        /// Returns <see cref="OkObjectResult"/> with the organisations if found; otherwise, an empty collection.
        /// </returns>
        [HttpGet("{userId}/organisations")]
        public async Task<IActionResult> GetUserOrganisations(Guid userId)
        {
            _logger.LogInformation("Get organisations for user with ID {UserId}", userId);
            IEnumerable<Organisation> organisations = await _organisationRepository.GetOrganisationsByUserIdAsync(userId).ConfigureAwait(false);

            _logger.LogInformation("{OrganisationCount} Organisations found for user with ID {UserId}", organisations.Count(), userId);
            return Ok(organisations);
        }

        /// <summary>
        /// Retrieves a user by their identity provider and identity provider user ID.
        /// </summary>
        /// <param name="identityProviderId">The identity provider's unique identifier.</param>
        /// <param name="identityProviderUserId">The user ID provided by the identity provider.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user if found; otherwise, <see cref="NotFoundResult"/>.
        /// </returns>
        [HttpGet("identity/{identityProviderId}/{identityProviderUserId}")]
        public async Task<IActionResult> GetUserByIdentityProviderId(Guid identityProviderId, string identityProviderUserId)
        {
            _logger.LogInformation("Get user with Identity Provider ID {IdentityProviderId} and User ID {IdentityProviderUserId}", identityProviderId, identityProviderUserId);
            User? user = await _userRepository.GetUserByIdentityProviderUserIdAsync(identityProviderUserId, identityProviderId).ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found with Identity Provider ID {IdentityProviderId} and User ID {IdentityProviderUserId}", identityProviderId, identityProviderUserId);
                return NotFound("User not found");
            }

            _logger.LogInformation("User found with Identity Provider ID {IdentityProviderId} and User ID {IdentityProviderUserId}", identityProviderId, identityProviderUserId);
            return Ok(user);
        }

        /// <summary>
        /// Retrieves all users from the data store.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of <see cref="User"/> entities.
        /// Returns <see cref="OkObjectResult"/> with the users if found; otherwise, an empty collection.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Get all users");
            IEnumerable<User> users = await _userRepository.GetUsersAsync().ConfigureAwait(false);

            _logger.LogInformation("{UserCount} Users found", users.Count());
            return Ok(users);
        }

        /// <summary>
        /// Creates a new user in the data store.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to create.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedAtActionResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            _logger.LogInformation("Create a user with ID {UserId}", user.Id);

            Helpers.Auditing.SetCreatedInfo(user, ControllerContext);

            bool result = await _userRepository.AddUserAsync(user).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to create user {Email}", user.Email);
                return StatusCode(500, "Failed to create user.");
            }

            _logger.LogInformation("User created with ID {UserId} {Email}", user.Id, user.Email);
            return CreatedAtAction(nameof(Get), new { userId = user.Id }, user);
        }

        /// <summary>
        /// Updates an existing user in the data store.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to update.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="OkObjectResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            _logger.LogInformation("Update user with ID {UserId}", user.Id);

            Helpers.Auditing.SetModifiedInfo(user, ControllerContext);

            bool result = await _userRepository.UpdateUserAsync(user).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to update user with ID {UserId}", user.Id);
                return StatusCode(500, "Failed to update user.");
            }

            _logger.LogInformation("User updated with ID {UserId} {Email}", user.Id, user.Email);
            return Ok(user);
        }

        /// <summary>
        /// Deletes a user from the data store by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; otherwise, <see cref="NotFoundResult"/> or <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(Guid userId)
        {
            //TODO: Check for related entities before allowing delete
            // Should we just mark as inactive instead e.g. IsDeleted?

            _logger.LogInformation("Delete user with ID {UserId}", userId);

            User? user = await _userRepository.GetUserByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User not found with ID {UserId}", userId);
                return NotFound("User not found");
            }

            bool result = await _userRepository.DeleteUserAsync(user).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError("Failed to delete user with ID {UserId}", userId);
                return StatusCode(500, "Failed to delete user.");
            }

            _logger.LogInformation("User deleted with ID {UserId}", userId);
            return NoContent();
        }
    }
}
