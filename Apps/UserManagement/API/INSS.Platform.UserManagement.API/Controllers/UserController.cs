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
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging operations.</param>
        /// <param name="userRepository">The user repository for data access.</param>
        public UserController(ILogger<UserController> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the user if found; otherwise, <see cref="NotFoundResult"/>.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            _logger.LogInformation("Get user with id: {UserId}", id);
            User? user = await _userRepository.GetUserByIdAsync(id).ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found for id: {UserId}", id);
                return NotFound();
            }

            _logger.LogInformation("User found for id: {UserId}", id);
            return Ok(user);
        }

        /// <summary>
        /// Adds a new user to the data store.
        /// </summary>
        /// <param name="user">The <see cref="User"/> to add.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="CreatedAtActionResult"/> if successful; otherwise, <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            _logger.LogInformation("Post user with id: {UserId}", user.Id);

            Helpers.Auditing.SetCreatedInfo(user, ControllerContext);

            bool result = await _userRepository.AddUserAsync(user).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to add user: {Email}", user.Email);
                return StatusCode(500, "Failed to add user.");
            }

            _logger.LogInformation("User added: {UserId} {Email}", user.Id, user.Email);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
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
        public async Task<IActionResult> Put([FromBody] User user)
        {
            _logger.LogInformation("Put user with id: {UserId}", user.Id);

            Helpers.Auditing.SetModifiedInfo(user, ControllerContext);

            bool result = await _userRepository.UpdateUserAsync(user).ConfigureAwait(false);

            if (!result)
            {
                _logger.LogError("Failed to update user: {Email}", user.Email);
                return StatusCode(500, "Failed to update user.");
            }

            _logger.LogInformation("User updated: {UserId} {Email}", user.Id, user.Email);
            return Ok(user);
        }

        /// <summary>
        /// Deletes a user from the data store by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the operation.
        /// Returns <see cref="NoContentResult"/> if successful; otherwise, <see cref="NotFoundResult"/> or <see cref="StatusCodeResult"/>.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Delete user with id: {UserId}", id);

            User? user = await _userRepository.GetUserByIdAsync(id).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User not found for id: {UserId}", id);
                return NotFound();
            }

            bool result = await _userRepository.DeleteUserAsync(user).ConfigureAwait(false);
            if (!result)
            {
                _logger.LogError("Failed to delete user with id: {UserId}", id);
                return StatusCode(500, "Failed to delete user.");
            }

            _logger.LogInformation("User deleted: {UserId}", id);
            return NoContent();
        }
    }
}
