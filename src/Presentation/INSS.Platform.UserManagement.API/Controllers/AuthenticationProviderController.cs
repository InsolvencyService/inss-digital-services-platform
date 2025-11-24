using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// Controller for managing authentication providers.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AuthenticationProviderController : ControllerBase
{
    private readonly ILogger<AuthenticationProviderController> _logger;
    private readonly IAuthenticationProviderRepository _authenticationProviderRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationProviderController"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="authenticationProviderRepository">The authentication provider repository.</param>
    public AuthenticationProviderController(ILogger<AuthenticationProviderController> logger, IAuthenticationProviderRepository authenticationProviderRepository)
    {
        _logger = logger;
        _authenticationProviderRepository = authenticationProviderRepository;
    }

    /// <summary>
    /// Gets an authentication provider by its unique identifier.
    /// </summary>
    /// <param name="authenticationProviderId">The unique identifier of the authentication provider.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The authentication provider if found; otherwise, a not found or error response.</returns>
    [HttpGet("{authenticationProviderId}")]
    public async Task<IActionResult> Get(Guid authenticationProviderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get authentication provider with ID {AuthenticationProviderId}", authenticationProviderId);

        OperationResult<AuthenticationProvider> result = await _authenticationProviderRepository.GetByIdAsync(authenticationProviderId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Authentication provider not found with ID {authenticationProviderId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all authentication providers.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of authentication providers or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all authentication providers");

        OperationResult<IEnumerable<AuthenticationProvider>> result = await _authenticationProviderRepository.GetAsync(cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new authentication provider.
    /// </summary>
    /// <param name="authenticationProvider">The authentication provider to create.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The created authentication provider or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AuthenticationProvider authenticationProvider, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create an authentication provider with ID {AuthenticationProviderId}", authenticationProvider.Id);

        Helpers.Auditing.SetCreatedInfo(authenticationProvider, ControllerContext);

        OperationResult<AuthenticationProvider> result = await _authenticationProviderRepository.AddAsync(authenticationProvider, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { authenticationProviderId = authenticationProvider.Id }, authenticationProvider)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing authentication provider.
    /// </summary>
    /// <param name="authenticationProvider">The authentication provider to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The updated authentication provider or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AuthenticationProvider authenticationProvider, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update an authentication provider with ID {AuthenticationProviderId}", authenticationProvider.Id);

        Helpers.Auditing.SetModifiedInfo(authenticationProvider, ControllerContext);

        OperationResult<AuthenticationProvider> result = await _authenticationProviderRepository.UpdateAsync(authenticationProvider, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(authenticationProvider)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes an authentication provider by its unique identifier.
    /// </summary>
    /// <param name="authenticationProviderId">The unique identifier of the authentication provider to delete.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{authenticationProviderId}")]
    public async Task<IActionResult> Delete(Guid authenticationProviderId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete authentication provider with ID {AuthenticationProviderId}", authenticationProviderId);

        OperationResult<AuthenticationProvider> result = await _authenticationProviderRepository.DeleteAsync(authenticationProviderId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Authentication provider not found with ID {authenticationProviderId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
