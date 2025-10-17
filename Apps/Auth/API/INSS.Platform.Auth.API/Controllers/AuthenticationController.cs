using INSS.Platform.Auth.API.Dto;
using INSS.Platform.Auth.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

namespace INSS.Platform.Auth.API.Controllers
{
    /// <summary>
    /// Provides authentication endpoints for login, callback, and login URL generation.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="authService">The authentication service.</param>
        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Redirects the user to the login page, passing the user ID (optional) and client URL (required).
        /// </summary>
        /// <param name="clientUrl">The client application's URL to return to after login.</param>
        /// <param name="userId">The optional user ID.</param>
        /// <returns>A redirect to the login URL or an error response.</returns>
        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string clientUrl, string userId = "")
        {
            _logger.LogInformation("Redirect to Login Login Page and pass the UserId (optional):{UserId} and ClientUrl(required):{ClientUrl} so that the process will seamlessly return to the client application after login and token request.", userId, clientUrl);

            (bool success, IActionResult actionResult, string loginUrl) = await TryGetLoginRedirectUrl(clientUrl, userId).ConfigureAwait(false);
            
            return success 
                ? Redirect(loginUrl)  
                : actionResult;
        }

        /// <summary>
        /// Gets the login URL, including the user ID (optional) and client URL (required).
        /// </summary>
        /// <param name="clientUrl">The client application's URL to return to after login.</param>
        /// <param name="userId">The optional user ID.</param>
        /// <returns>The login URL or an error response.</returns>
        [HttpGet("login-url")]
        public async Task<IActionResult> GetLoginUrl([FromQuery] string clientUrl, string userId = "")
        {
            _logger.LogInformation("Get Login Url which will include the UserId (optional):{UserId} and ClientUrl(required):{ClientUrl} so that the process will seamlessly return to the client application after login and token request.", userId, clientUrl);

            (bool success, IActionResult actionResult, string loginUrl) = await TryGetLoginRedirectUrl(clientUrl, userId).ConfigureAwait(false);

            return success
                ? Ok(loginUrl)
                : actionResult;
        }

        /// <summary>
        /// Handles the authentication callback from the login provider.
        /// </summary>
        /// <param name="code">The authorization code received from the provider.</param>
        /// <param name="state">The state value used to correlate the request.</param>
        /// <returns>A redirect to the client application or an error response.</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> CallBack([FromQuery] string? code, string? state)
        {
            _logger.LogInformation("Callback from Login, code: {Code} - state: {State}", code, state);

            (bool isValid, IActionResult? value) = ValidateCallbackParameters(code, state);
            if (!isValid)
            {
                return value;
            }

            (bool isStateValid, string nonce, string csrfToken, string userId, string clientUrl) = await _authService.ValidateAndExtractStateAsync(state!).ConfigureAwait(false);
            // NOTE: We currently do not have a way to validate the csrfToken here because we do not have access to a state cache or the user's session or cookies.
            // Waiting on Wayne to authorize the design for CSRF protection in this flow. 
            _logger.LogInformation("Extracted state parameters - Nonce: {Nonce}, CSRF Token: {CsrfToken}, UserId: {UserId}, ClientUrl: {ClientUrl}", nonce, csrfToken, userId, clientUrl);
            if (!isStateValid)
            {
                _logger.LogError("Invalid state parameter in the callback request.");
                return BadRequest("Invalid state parameter.");
            }

            TokenData tokenData;
            try
            {
                tokenData = await _authService.HandleCallbackAsync(code!, nonce).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling authentication callback.");
                return StatusCode(500, "Authentication callback failed.");
            }

            Uri clientUri = BuildClientUrl(clientUrl, userId, tokenData);
            return Redirect(clientUri.ToString());
        }

        /// <summary>
        /// Builds the client URL with authentication tokens and user information as query parameters.
        /// </summary>
        /// <param name="requestState">The request state containing client URL and user ID.</param>
        /// <param name="tokenData">The token data to include in the URL.</param>
        /// <returns>The constructed client URL.</returns>
        private static Uri BuildClientUrl(string clientUrl, string userId, TokenData tokenData)
        {
            UriBuilder uriBuilder = new(clientUrl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (!string.IsNullOrEmpty(userId))
            {
                query["userId"] = userId;
            }

            if (tokenData != null)
            {
                query["accessToken"] = tokenData.AccessToken;
                query["idToken"] = tokenData.IdToken;
                query["expiresIn"] = tokenData.ExpiresIn.ToString(CultureInfo.InvariantCulture);
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Attempts to generate the login redirect URL using the client URL and user ID.
        /// </summary>
        /// <param name="clientUrl">The client application's URL.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="loginUrl">The generated login URL.</param>
        /// <returns>A tuple indicating success and the corresponding action result.</returns>
        private async Task<(bool result, IActionResult actionResult, string loginUrl)> TryGetLoginRedirectUrl(string clientUrl, string userId)
        {
            string loginUrl;

            try
            {
                loginUrl = await _authService.GetLoginRedirectUrl(clientUrl, userId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating login url.");
                return (false, StatusCode(500, "login url generation failed."), string.Empty);
            }

            return (true, Ok(), loginUrl);
        }

        /// <summary>
        /// Validates the callback parameters received from the authentication provider.
        /// </summary>
        /// <param name="code">The authorization code received from the provider.</param>
        /// <param name="state">The state value used to correlate the request.</param>
        /// <returns>
        /// A tuple containing:
        /// <c>isValid</c>: <c>true</c> if both parameters are present; otherwise, <c>false</c>.
        /// <c>actionResult</c>: An <see cref="IActionResult"/> representing the validation result.
        /// </returns>
        private (bool isValid, IActionResult actionResult) ValidateCallbackParameters(string? code, string? state)
        {
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogError("Authorization code is missing in the callback request.");
                return (false, BadRequest("Authorization code is missing."));
            }

            if (string.IsNullOrEmpty(state))
            {
                _logger.LogError("State is missing in the callback request.");
                return (false, BadRequest("State code is missing."));
            }

            return (true, Ok());
        }
    }
}
