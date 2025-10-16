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
        private readonly IStateCache _stateCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="authService">The authentication service.</param>
        /// <param name="stateCache">The state cache service.</param>
        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthService authService, IStateCache stateCache)
        {
            _logger = logger;
            _authService = authService;
            _stateCache = stateCache;
        }

        /// <summary>
        /// Redirects the user to the login page, passing the user ID (optional) and client URL (required).
        /// </summary>
        /// <param name="clientUrl">The client application's URL to return to after login.</param>
        /// <param name="userId">The optional user ID.</param>
        /// <returns>A redirect to the login URL or an error response.</returns>
        [HttpGet("login")]
        public IActionResult Login([FromQuery] string clientUrl, string userId = "")
        {
            _logger.LogInformation("Redirect to Login Login Page and pass the UserId (optional):{UserId} and ClientUrl(required):{ClientUrl} so that the process will seamlessly return to the client application after login and token request.", userId, clientUrl);

            (bool success, IActionResult actionResult) = TryGetLoginRedirectUrl(clientUrl, userId, out string loginUrl);
            
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
        public IActionResult GetLoginUrl([FromQuery] string clientUrl, string userId = "")
        {
            _logger.LogInformation("Get Login Url which will include the UserId (optional):{UserId} and ClientUrl(required):{ClientUrl} so that the process will seamlessly return to the client application after login and token request.", userId, clientUrl);

            (bool success, IActionResult actionResult) = TryGetLoginRedirectUrl(clientUrl, userId, out string loginUrl);

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

            (bool isValid, IActionResult? value) = ValidateCallbackParameters(code, state, out RequestState? requestState);
            if (!isValid)
            {
                return value;
            }

            TokenData tokenData;
            try
            {
                tokenData = await _authService.HandleCallbackAsync(code!).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling authentication callback.");
                return StatusCode(500, "Authentication callback failed.");
            }

            Uri clientUrl = BuildClientUrl(requestState!, tokenData);
            return Redirect(clientUrl.ToString());
        }

        /// <summary>
        /// Stores the client URL and user ID in the state cache and returns the generated state key.
        /// </summary>
        /// <param name="clientUrl">The client application's URL.</param>
        /// <param name="userId">The user ID.</param>
        /// <returns>The generated state key.</returns>
        private string SetStateCache(string clientUrl, string userId)
        {
            string stateKey = Guid.NewGuid().ToString("N");
            RequestState requestState = new()
            {
                ClientUrl = clientUrl,
                UserId = userId
            };

            _stateCache.Store(stateKey.ToString(), requestState);

            return stateKey;
        }

        /// <summary>
        /// Builds the client URL with authentication tokens and user information as query parameters.
        /// </summary>
        /// <param name="requestState">The request state containing client URL and user ID.</param>
        /// <param name="tokenData">The token data to include in the URL.</param>
        /// <returns>The constructed client URL.</returns>
        private static Uri BuildClientUrl(RequestState requestState, TokenData tokenData)
        {
            UriBuilder uriBuilder = new(requestState.ClientUrl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (!string.IsNullOrEmpty(requestState.UserId))
            {
                query["userId"] = requestState.UserId;
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
        private (bool result, IActionResult actionResult) TryGetLoginRedirectUrl(string clientUrl, string userId, out string loginUrl)
        {
            loginUrl = string.Empty;

            try
            {
                string stateCacheKey = SetStateCache(clientUrl, userId);
                loginUrl = _authService.GetLoginRedirectUrl(stateCacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating login url.");
                return (result: false, actionResult: StatusCode(500, "login url generation failed."));
            }

            return (result: true, actionResult: Ok());
        }

        /// <summary>
        /// Validates the callback parameters and retrieves the associated request state from the cache.
        /// </summary>
        /// <param name="code">The authorization code.</param>
        /// <param name="state">The state key.</param>
        /// <param name="requestState">The retrieved request state object.</param>
        /// <returns>A tuple indicating validity and the corresponding action result.</returns>
        private (bool isValid, IActionResult actionResult) ValidateCallbackParameters(string? code, string? state, out RequestState? requestState)
        {
            requestState = null;

            if (string.IsNullOrEmpty(code))
            {
                _logger.LogError("Authorization code is missing in the callback request.");
                return (isValid: false, actionResult: BadRequest("Authorization code is missing."));
            }

            if (string.IsNullOrEmpty(state))
            {
                _logger.LogError("State is missing in the callback request.");
                return (isValid: false, actionResult: BadRequest("State code is missing."));
            }

            if (!_stateCache.TryGet(state, out requestState) || requestState == null)
            {
                _logger.LogError("State object not found in cache with Key: {State}", state);
                return (isValid: false, actionResult: BadRequest("State object not found in cache."));
            }

            return (isValid: true, actionResult: Ok());
        }
    }
}
