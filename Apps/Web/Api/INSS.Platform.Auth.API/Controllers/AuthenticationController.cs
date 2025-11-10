using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.Contracts;
using INSS.Platform.Auth.Contracts.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace INSS.Platform.Auth.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly AuthenticationProviderOptions _options;
        private const string ReturnUrlKey = "returnUrl";
        private const string UserIdKey = "userId";

        public AuthenticationController(ILogger<AuthenticationController> logger, IOptions<AuthenticationProviderOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        [HttpGet("{provider}/signin")]
        public IActionResult SignIn(AuthenticationProvider provider, 
            [FromQuery] SignInRequest signInRequest)
        {
            _logger.LogInformation("Begin SignIn process for Provider:{Provider} - ClientRedirectUrl:{ClientRedirectUrl} - UserId:{UserId}", provider.ToString(), signInRequest.ClientRedirectUrl, signInRequest.UserId);

            if (ValidateClientRedirectUrl(signInRequest.ClientRedirectUrl) is { Valid: false } result)
            {
                return result.ActionResult!;
            }

            AuthenticationProperties properties = new(new Dictionary<string, string?>(1)
            {
                { ReturnUrlKey, signInRequest.ClientRedirectUrl }
            });

            if (!string.IsNullOrEmpty(signInRequest.UserId))
            {
                properties.Items.Add(UserIdKey, signInRequest.UserId);
            }

            // Raise the orchestrated set of provider events starting with: OnAuthorizationCodeReceived.
            return Challenge(properties, provider.ToString());
        }

        [HttpGet("{provider}/signout")]
        public async Task<IActionResult> SignOut(AuthenticationProvider provider)
        {
            _logger.LogInformation("Begin SignOut process for Provider:{Provider}", provider.ToString());

            // Raise the OnRedirectToIdentityProviderForSignOut Event for the provider.
            return SignOut(provider.ToString());
        }

        [HttpGet("post-signout")]
        public async Task<IActionResult> PostSignOut()
        {
            _logger.LogInformation("Begin PostSignOut process after being called back from the sign-out provider.");

            AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties? properties = result.Properties;

            string clientReturnUrl;
            if (properties?.Items.TryGetValue(ReturnUrlKey, out string? value) == true)
            {
                clientReturnUrl = value!;
            }
            else
            {
                // This occurs when signing out of Entra as you are initially prompted with a list of accounts to sign out of.
                // A second call is then made which and provides the returnUrl.
                _logger.LogInformation("{ReturnUrl} is not available in AuthenticationProperties.", ReturnUrlKey);
                return Ok();
            }

            // Clear out any remaining cookies
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect(clientReturnUrl);
        }

        /// <summary>
        /// Validates the client redirect URL against the list of allowed URLs.
        /// Only the base URL (scheme + host + port) is validated. Path and query components are ignored.
        /// </summary>
        /// <param name="clientRedirectUrl">The client redirect URL to validate.</param>
        /// <returns>
        /// A tuple indicating whether the URL is valid and, if not valid, an <see cref="IActionResult"/> describing the error.
        /// </returns>
        private (bool Valid, IActionResult? ActionResult) ValidateClientRedirectUrl(string clientRedirectUrl)
        {
            Uri absoluteUrl = new(clientRedirectUrl, UriKind.Absolute);
            string baseUrl = absoluteUrl.GetLeftPart(UriPartial.Authority);

            bool isAllowed = _options.AllowedClientRedirectUrls
                .Any(allowed => string.Equals(allowed.TrimEnd('/'), baseUrl, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
            {
                _logger.LogError(
                    "Invalid ClientRedirectUrl {ClientRedirectUrl} (base: {BaseUrl}) does not exist in the list of allowed client redirect url's: {AllowedClientRedirectUrls}",
                    clientRedirectUrl,
                    baseUrl,
                    string.Join(",", _options.AllowedClientRedirectUrls)
                );

                return (Valid: false, ActionResult: BadRequest("Invalid ClientRedirectUrl"));
            }

            return (Valid: true, ActionResult: null);
        }
    }
}
