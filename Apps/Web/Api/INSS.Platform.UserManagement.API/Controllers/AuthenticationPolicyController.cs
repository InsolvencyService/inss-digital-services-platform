using INSS.Platform.UserManagement.Abstractions.Repositories;
using INSS.Platform.UserManagement.Abstractions.Results;
using INSS.Platform.UserManagement.Entities;
using Microsoft.AspNetCore.Mvc;

namespace INSS.Platform.UserManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing authentication policies.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationPolicyController : ControllerBase
    {
        private readonly ILogger<AuthenticationPolicyController> _logger;
        private readonly IAuthenticationPolicyRepository _authenticationPolicyRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationPolicyController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="authenticationPolicyRepository">The authentication policy repository.</param>
        public AuthenticationPolicyController(ILogger<AuthenticationPolicyController> logger, IAuthenticationPolicyRepository authenticationPolicyRepository)
        {
            _logger = logger;
            _authenticationPolicyRepository = authenticationPolicyRepository;
        }

        /// <summary>
        /// Gets an authentication policy by its unique identifier.
        /// </summary>
        /// <param name="authenticationPolicyId">The unique identifier of the authentication policy.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The authentication policy if found; otherwise, a not found or error response.</returns>
        [HttpGet("{authenticationPolicyId}")]
        public async Task<IActionResult> Get(Guid authenticationPolicyId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get authentication policy with ID {AuthenticationPolicyId}", authenticationPolicyId);
            
            OperationResult<AuthenticationPolicy> result = await _authenticationPolicyRepository.GetByIdAsync(authenticationPolicyId, cancellationToken).ConfigureAwait(false);

            if (!result.Success)
            {
                if(result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Authentication policy not found with ID {authenticationPolicyId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return Ok(result.Entity);
        }

        /// <summary>
        /// Gets all authentication policies.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A list of authentication policies or an error response.</returns>
        [HttpGet()]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Get all authentication policies");

            OperationResult<IEnumerable<AuthenticationPolicy>> result = await _authenticationPolicyRepository.GetAsync(cancellationToken).ConfigureAwait(false);

            return result.Success
                ? Ok(result.Entity)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Creates a new authentication policy.
        /// </summary>
        /// <param name="authenticationPolicy">The authentication policy to create.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The created authentication policy or an error response.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AuthenticationPolicy authenticationPolicy, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Create an authentication policy with ID {AuthenticationPolicyId}", authenticationPolicy.Id);

            Helpers.Auditing.SetCreatedInfo(authenticationPolicy, ControllerContext);

            OperationResult<AuthenticationPolicy> result = await _authenticationPolicyRepository.AddAsync(authenticationPolicy, cancellationToken).ConfigureAwait(false);

            return result.Success
                ? CreatedAtAction(nameof(Get), new { authenticationPolicyId = authenticationPolicy.Id }, authenticationPolicy)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Updates an existing authentication policy.
        /// </summary>
        /// <param name="authenticationPolicy">The authentication policy to update.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The updated authentication policy or an error response.</returns>
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AuthenticationPolicy authenticationPolicy, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update an authentication policy with ID {AuthenticationPolicyId}", authenticationPolicy.Id);

            Helpers.Auditing.SetModifiedInfo(authenticationPolicy, ControllerContext);

            OperationResult<AuthenticationPolicy> result = await _authenticationPolicyRepository.UpdateAsync(authenticationPolicy, cancellationToken).ConfigureAwait(false);

            return result.Success
                ? Ok(authenticationPolicy)
                : StatusCode(500, result.ErrorMessage);
        }

        /// <summary>
        /// Deletes an authentication policy by its unique identifier.
        /// </summary>
        /// <param name="authenticationPolicyId">The unique identifier of the authentication policy to delete.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>No content if deleted; otherwise, a not found or error response.</returns>
        [HttpDelete("{authenticationPolicyId}")]
        public async Task<IActionResult> Delete(Guid authenticationPolicyId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Delete authentication policy with ID {AuthenticationPolicyId}", authenticationPolicyId);

            OperationResult<AuthenticationPolicy> result = await _authenticationPolicyRepository.DeleteAsync(authenticationPolicyId, cancellationToken).ConfigureAwait(false);

            if (!result.Success)
            {
                if (result.ErrorCode == OperationErrorCode.NotFound)
                {
                    return NotFound($"Authentication policy not found with ID {authenticationPolicyId}");
                }

                return StatusCode(500, result.ErrorMessage);
            }

            return NoContent();
        }
    }
}
