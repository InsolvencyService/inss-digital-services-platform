using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// Controller for managing authentication policy providers.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AuthenticationPolicyProviderController : ControllerBase
{
    private readonly ILogger<AuthenticationPolicyProviderController> _logger;
    private readonly IAuthenticationPolicyProviderRepository _authenticationPolicyProviderRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationPolicyProviderController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="authenticationPolicyProviderRepository">The authentication policy provider repository.</param>
    public AuthenticationPolicyProviderController(
        ILogger<AuthenticationPolicyProviderController> logger,
        IAuthenticationPolicyProviderRepository authenticationPolicyProviderRepository)
    {
        _logger = logger;
        _authenticationPolicyProviderRepository = authenticationPolicyProviderRepository;
    }

    /// <summary>
    /// Gets an authentication policy provider by its unique identifier.
    /// </summary>
    /// <param name="authenticationPolicyProviderId">The unique identifier of the authentication policy provider.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The authentication policy provider if found; otherwise, a not found or error response.</returns>
    [HttpGet("{authenticationPolicyProviderId}")]
    public async Task<IActionResult> Get(Guid authenticationPolicyProviderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get authentication policy provider with ID {AuthenticationPolicyProviderId}", authenticationPolicyProviderId);

        OperationResult<AuthenticationPolicyProvider> result = await _authenticationPolicyProviderRepository.GetByIdAsync(authenticationPolicyProviderId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Authentication policy provider not found with ID {authenticationPolicyProviderId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all authentication policy providers.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of authentication policy providers or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all authentication policy providers");

        OperationResult<IEnumerable<AuthenticationPolicyProvider>> result = await _authenticationPolicyProviderRepository.GetAsync(cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new authentication policy provider.
    /// </summary>
    /// <param name="authenticationPolicyProvider">The authentication policy provider to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created authentication policy provider or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AuthenticationPolicyProvider authenticationPolicyProvider, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create an authentication policy provider with ID {AuthenticationPolicyProviderId}", authenticationPolicyProvider.Id);

        Helpers.Auditing.SetCreatedInfo(authenticationPolicyProvider, ControllerContext);

        OperationResult<AuthenticationPolicyProvider> result = await _authenticationPolicyProviderRepository.AddAsync(authenticationPolicyProvider, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { authenticationPolicyProviderId = authenticationPolicyProvider.Id }, authenticationPolicyProvider)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing authentication policy provider.
    /// </summary>
    /// <param name="authenticationPolicyProvider">The authentication policy provider to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated authentication policy provider or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AuthenticationPolicyProvider authenticationPolicyProvider, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update an authentication policy provider with ID {AuthenticationPolicyProviderId}", authenticationPolicyProvider.Id);

        Helpers.Auditing.SetModifiedInfo(authenticationPolicyProvider, ControllerContext);

        OperationResult<AuthenticationPolicyProvider> result = await _authenticationPolicyProviderRepository.UpdateAsync(authenticationPolicyProvider, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(authenticationPolicyProvider)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes an authentication policy provider by its unique identifier.
    /// </summary>
    /// <param name="authenticationPolicyProviderId">The unique identifier of the authentication policy provider to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{authenticationPolicyProviderId}")]
    public async Task<IActionResult> Delete(Guid authenticationPolicyProviderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete authentication policy provider with ID {AuthenticationPolicyProviderId}", authenticationPolicyProviderId);

        OperationResult<AuthenticationPolicyProvider> result = await _authenticationPolicyProviderRepository.DeleteAsync(authenticationPolicyProviderId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Authentication policy provider not found with ID {authenticationPolicyProviderId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
