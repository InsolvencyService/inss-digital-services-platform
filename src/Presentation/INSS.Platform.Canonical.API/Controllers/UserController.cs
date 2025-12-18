using INSS.Platform.Canonical.Application.Repositories;
using INSS.Platform.Canonical.Application.Results;
using INSS.Platform.Canonical.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.Canonical.API.Controllers;

/// <summary>
/// API controller for managing user entities.
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
    /// <param name="logger">Logger instance for logging operations.</param>
    /// <param name="UserRepository">Repository for User data access.</param>
    public UserController(ILogger<UserController> logger, IUserRepository UserRepository)
    {
        _logger = logger;
        _userRepository = UserRepository;
    }

    /// <summary>
    /// Gets an user by its unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The user entity if found; otherwise, a not found or error response.</returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get user with ID {UserId}", userId);
        
        OperationResult<User> result = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"User not found with ID {userId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>A collection of user entities or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all users");
        
        OperationResult<IEnumerable<User>> result = await _userRepository.GetAsync(cancellationToken);

        return result.Success 
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user entity to create.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The created user entity or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] User user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create an user with ID {UserId}", user.Id);

        Helpers.Auditing.SetCreatedInfo(user, ControllerContext);

        OperationResult<User> result = await _userRepository.AddAsync(user, cancellationToken);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { userId = user.Id }, user)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    /// <param name="user">The user entity to update.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The updated user entity or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] User user, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update an user with ID {UserId}", user.Id);

        Helpers.Auditing.SetModifiedInfo(user, ControllerContext);

        OperationResult<User> result = await _userRepository.UpdateAsync(user, cancellationToken);

        return result.Success
            ? Ok(user)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes an user by its unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>No content if successful; otherwise, a not found or error response.</returns>
    [HttpDelete("{userId}")]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete user with ID {UserId}", userId);

        OperationResult<User> result = await _userRepository.DeleteAsync(userId, cancellationToken);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"User not found with ID {userId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
