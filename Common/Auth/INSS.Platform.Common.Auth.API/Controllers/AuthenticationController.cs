using INSS.Platform.Common.Auth.API.Services;
using INSS.Platform.Common.Auth.Contracts.Request;
using INSS.Platform.Common.Auth.Contracts.Response;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Web;

namespace INSS.Platform.Common.Auth.API.Controllers
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
        /// Initiates the login process by redirecting the user to the authentication provider's login page.
        /// </summary>
        /// <param name="loginRequest">
        /// The <see cref="LoginRequest"/> containing the optional UserId and required ClientUrl.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects to the login URL if successful, or an error response if not.
        /// </returns>
        /// <remarks>
        /// This endpoint is used by client applications to start the authentication flow.
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("Redirect to Login Login Page and pass the UserId (optional):{UserId} and ClientUrl(required):{ClientUrl} so that the process will seamlessly return to the client application after login and token request.", loginRequest.UserId, loginRequest.ClientUrl);

            (bool success, IActionResult actionResult, string loginUrl) = await TryGetLoginRedirectUrlAsync(loginRequest).ConfigureAwait(false);
            
            return success 
                ? Redirect(loginUrl)  
                : actionResult;
        }

        /// <summary>
        /// Generates the login URL for the authentication provider based on the specified login request.
        /// </summary>
        /// <param name="loginRequest">
        /// The <see cref="LoginRequest"/> containing the optional UserId and required ClientUrl.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the login URL if successful, or an error response if not.
        /// </returns>
        /// <remarks>
        /// This endpoint is used by client applications to retrieve the login URL for manual redirection.
        /// </remarks>
        [HttpPost("login-url")]
        public async Task<IActionResult> LoginUrlAsync([FromBody] LoginRequest loginRequest)
        {
            _logger.LogInformation("Get Login Url which will include the UserId (optional):{UserId} and ClientUrl(required):{ClientUrl} so that the process will seamlessly return to the client application after login and token request.", loginRequest.UserId, loginRequest.ClientUrl);

            (bool success, IActionResult actionResult, string loginUrl) = await TryGetLoginRedirectUrlAsync(loginRequest).ConfigureAwait(false);

            return success
                ? Ok(loginUrl)
                : actionResult;
        }

        /// <summary>
        /// Logs out the user by invalidating their authentication session.
        /// </summary>
        /// <param name="logoutRequest">
        /// The <see cref="LogoutRequest"/> containing the post-logout redirect URI and any additional logout parameters.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the logout operation:
        /// <list type="bullet">
        /// <item>
        /// <description><c>Ok</c> if logout was successful.</description>
        /// </item>
        /// <item>
        /// <description><c>StatusCode(500)</c> if logout failed.</description>
        /// </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// This endpoint should be called by client applications to log out the user and optionally redirect them after logout.
        /// </remarks>
        [HttpPost("logout")]
        public async Task<IActionResult> LogOutAsync([FromBody] LogoutRequest logoutRequest)
        {
            _logger.LogInformation("Processing logout request");

            bool logoutSuccess = await _authService.LogoutAsync(logoutRequest).ConfigureAwait(false);

            return logoutSuccess
                ? Ok("Logout successful.")
                : StatusCode(500, "Logout failed.");
        }


        /// <summary>
        /// Handles the authentication callback from the login provider.
        /// </summary>
        /// <param name="code">The authorization code received from the provider.</param>
        /// <param name="state">The state value used to correlate the request.</param>
        /// <returns>A redirect to the client application or an error response.</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> CallBackAsync([FromQuery] string? code, string? state)
        {
            _logger.LogInformation("Callback from Login, code: {Code} - state: {State}", code, state);

            (bool isValid, IActionResult? value) = ValidateCallbackParameters(code, state);
            if (!isValid)
            {
                return value;
            }

            (bool isStateValid, string nonce, string csrfToken, string userId, string clientUrl) = await _authService.ValidateAndExtractRequestStateAsync(state!).ConfigureAwait(false);
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

            Uri clientUri = BuildClientUrl(csrfToken, clientUrl, userId, tokenData);
            return Redirect(clientUri.ToString());
        }

        /// <summary>
        /// Builds the client application's redirect URL with authentication tokens and parameters.
        /// </summary>
        /// <param name="csrfToken">
        /// The CSRF token to include in the query string for security validation.
        /// </param>
        /// <param name="clientUrl">
        /// The base URL of the client application to redirect to after authentication.
        /// </param>
        /// <param name="userId">
        /// The user ID to include in the query string, if available.
        /// </param>
        /// <param name="tokenData">
        /// The <see cref="TokenData"/> containing the access and ID tokens to include in the query string.
        /// </param>
        /// <returns>
        /// A <see cref="Uri"/> representing the client application's redirect URL with all required query parameters.
        /// </returns>
        private static Uri BuildClientUrl(string csrfToken, string clientUrl, string userId, TokenData tokenData)
        {
            UriBuilder uriBuilder = new(clientUrl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            query["csrf_token"] = csrfToken;

            if (!string.IsNullOrEmpty(userId))
            {
                query["user_id"] = userId;
            }

            if (tokenData != null)
            {
                query["access_token"] = tokenData.AccessToken;
                query["id_token"] = tokenData.IdToken;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Attempts to generate the login redirect URL for the authentication provider based on the specified login request.
        /// </summary>
        /// <param name="loginRequest">
        /// The <see cref="LoginRequest"/> containing the CSRF token, client URL, and optional user ID.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a tuple with:
        /// <list type="bullet">
        /// <item>
        /// <description><c>result</c>: <c>true</c> if the login URL was generated successfully; otherwise, <c>false</c>.</description>
        /// </item>
        /// <item>
        /// <description><c>actionResult</c>: An <see cref="IActionResult"/> representing the HTTP response to return if unsuccessful.</description>
        /// </item>
        /// <item>
        /// <description><c>loginUrl</c>: The generated login URL as a string if successful; otherwise, an empty string.</description>
        /// </item>
        /// </list>
        /// </returns>
        private async Task<(bool result, IActionResult actionResult, string loginUrl)> TryGetLoginRedirectUrlAsync(LoginRequest loginRequest)
        {
            string loginUrl;

            try
            {
                loginUrl = await _authService.GetLoginRedirectUrlAsync(loginRequest).ConfigureAwait(false);
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
