using INSS.Platform.UserManagement.Application.Repositories;
using INSS.Platform.UserManagement.Application.Results;
using INSS.Platform.UserManagement.Domain;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers;

/// <summary>
/// Controller for managing authentication provider metadata.
/// </summary>
[Route("[controller]")]
[ApiController]
public class AuthenticationProviderMetadataController : ControllerBase
{
    private readonly ILogger<AuthenticationProviderMetadataController> _logger;
    private readonly IAuthenticationProviderMetadataRepository _authenticationProviderMetadataRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationProviderMetadataController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance.</param>
    /// <param name="authenticationProviderMetadataRepository">Repository for authentication provider metadata.</param>
    public AuthenticationProviderMetadataController(
        ILogger<AuthenticationProviderMetadataController> logger,
        IAuthenticationProviderMetadataRepository authenticationProviderMetadataRepository)
    {
        _logger = logger;
        _authenticationProviderMetadataRepository = authenticationProviderMetadataRepository;
    }

    /// <summary>
    /// Gets the authentication provider metadata by its unique identifier.
    /// </summary>
    /// <param name="authenticationProviderMetadataId">The unique identifier of the authentication provider metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The authentication provider metadata if found; otherwise, a not found or error response.</returns>
    [HttpGet("{authenticationProviderMetadataId}")]
    public async Task<IActionResult> Get(Guid authenticationProviderMetadataId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get authentication provider metadata with ID {AuthenticationProviderMetadataId}", authenticationProviderMetadataId);

        OperationResult<AuthenticationProviderMetadata> result = await _authenticationProviderMetadataRepository.GetByIdAsync(authenticationProviderMetadataId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if(result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Authentication provider metadata not found with ID {authenticationProviderMetadataId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return Ok(result.Entity);
    }

    /// <summary>
    /// Gets all authentication provider metadata.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of authentication provider metadata or an error response.</returns>
    [HttpGet()]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get all authentication provider metadata");

        OperationResult<IEnumerable<AuthenticationProviderMetadata>> result = await _authenticationProviderMetadataRepository.GetAsync(cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(result.Entity)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Creates a new authentication provider metadata.
    /// </summary>
    /// <param name="authenticationProviderMetadata">The authentication provider metadata to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created authentication provider metadata or an error response.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AuthenticationProviderMetadata authenticationProviderMetadata, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create an authentication provider metadata with ID {AuthenticationProviderMetadataId}", authenticationProviderMetadata.Id);

        Helpers.Auditing.SetCreatedInfo(authenticationProviderMetadata, ControllerContext);

        OperationResult<AuthenticationProviderMetadata> result = await _authenticationProviderMetadataRepository.AddAsync(authenticationProviderMetadata, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? CreatedAtAction(nameof(Get), new { authenticationProviderMetadataId = authenticationProviderMetadata.Id }, authenticationProviderMetadata)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Updates an existing authentication provider metadata.
    /// </summary>
    /// <param name="authenticationProviderMetadata">The authentication provider metadata to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated authentication provider metadata or an error response.</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AuthenticationProviderMetadata authenticationProviderMetadata, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update an authentication provider metadata with ID {AuthenticationProviderMetadataId}", authenticationProviderMetadata.Id);

        Helpers.Auditing.SetModifiedInfo(authenticationProviderMetadata, ControllerContext);

        OperationResult<AuthenticationProviderMetadata> result = await _authenticationProviderMetadataRepository.UpdateAsync(authenticationProviderMetadata, cancellationToken).ConfigureAwait(false);

        return result.Success
            ? Ok(authenticationProviderMetadata)
            : StatusCode(500, result.ErrorMessage);
    }

    /// <summary>
    /// Deletes the authentication provider metadata by its unique identifier.
    /// </summary>
    /// <param name="authenticationProviderMetadataId">The unique identifier of the authentication provider metadata to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
    [HttpDelete("{authenticationProviderMetadataId}")]
    public async Task<IActionResult> Delete(Guid authenticationProviderMetadataId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete authentication provider metadata with ID {AuthenticationProviderMetadataId}", authenticationProviderMetadataId);

        OperationResult<AuthenticationProviderMetadata> result = await _authenticationProviderMetadataRepository.DeleteAsync(authenticationProviderMetadataId, cancellationToken).ConfigureAwait(false);

        if (!result.Success)
        {
            if (result.ErrorCode == OperationErrorCode.NotFound)
            {
                return NotFound($"Authentication provider metadata not found with ID {authenticationProviderMetadataId}");
            }

            return StatusCode(500, result.ErrorMessage);
        }

        return NoContent();
    }
}
