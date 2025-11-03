using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.Contracts;
using INSS.Platform.Auth.Contracts.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace INSS.Platform.Auth.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly AuthProviderOptions _options;


        public AuthenticationController(ILogger<AuthenticationController> logger, IOptions<AuthProviderOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        [HttpGet("{provider}/signin")]
        public IActionResult SignIn(AuthProvider provider, 
            [FromQuery] SignInRequest signInRequest)
        {
            _logger.LogInformation("Begin SignIn process for Provider:{Provider} - PostSignInRedirect:{PostSignInRedirectUri} - UserId:{UserId}", provider.ToString(), signInRequest.PostSignInRedirectUri, signInRequest.UserId);

            if (ValidatePostSignInRedirectUri(signInRequest) is { Valid: false } result)
            {
                return result.ActionResult!;
            }

            AuthenticationProperties properties = new(new Dictionary<string, string?>(1)
            {
                { "returnUrl", signInRequest.PostSignInRedirectUri }
            });

            if (!string.IsNullOrEmpty(signInRequest.UserId))
            {
                properties.Items.Add("userId", signInRequest.UserId);
            }

            // Raise the orchestrated set of provider events starting with: OnRedirectToIdentityProvider.
            return Challenge(properties, provider.ToString());
        }

        [HttpGet("{provider}/signout")]
        public async Task<IActionResult> SignOut(AuthProvider provider, 
            [FromQuery] SignOutRequest signOutRequest)
        {
            _logger.LogInformation("Begin SignOut process for Provider:{Provider} - PostSignOutRedirectUri:{PostSignOutRedirectUri}", provider.ToString(), signOutRequest.PostSignOutRedirectUri);

            if (ValidatePostSignOutRedirectUri(signOutRequest) is { Valid: false } result)
            {
                return result.ActionResult!;
            }

            AuthenticationProperties properties = new(new Dictionary<string, string?>(1)
            {
                { "returnUrl", signOutRequest.PostSignOutRedirectUri }
            });

            // Raise the OnRedirectToIdentityProviderForSignOut Event for the provider.
            return SignOut(properties, provider.ToString());
        }

        [HttpGet("post-signout")]
        public async Task<IActionResult> PostSignOut()
        {
            _logger.LogInformation("Begin PostSignOut process after being redirected from the provider.");

            AuthenticateResult result = await HttpContext.AuthenticateAsync("Cookies");
            AuthenticationProperties? properties = result.Properties;

            // Get the return url for the client that was stored in SignOut()
            string returnUrl = string.Empty;
            if (properties?.Items.TryGetValue("returnUrl", out string? value) == true && value is not null)
            {
                returnUrl = value;
            }

            // Clear out any remaining cookies
            await HttpContext.SignOutAsync("Cookies");

            return Redirect(returnUrl);
        }

        private (bool Valid, IActionResult? ActionResult) ValidatePostSignInRedirectUri(SignInRequest signInRequest)
        {
            if (!string.IsNullOrEmpty(signInRequest.PostSignInRedirectUri) && signInRequest.PostSignInRedirectUri.EndsWith('/'))
            {
                // Trim trailing slash to ensure that redirect URIs are compared correctly in both the provider and api configuration.
                signInRequest.PostSignInRedirectUri = signInRequest.PostSignInRedirectUri.TrimEnd('/');
            }

            bool isAllowed = _options.AllowedPostSignInRedirectUris.Any(allowed => string.Equals(allowed, signInRequest.PostSignInRedirectUri, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
            {
                _logger.LogError(
                    "Invalid PostSignOutRedirectUri. {PostSignInRedirectUri} does not exist in the list of allowed client redirect uri's: {PostSignInRedirectUris}",
                    signInRequest.PostSignInRedirectUri,
                    string.Join(",", _options.AllowedPostSignInRedirectUris)
                );

                return (Valid: false, ActionResult: BadRequest("Invalid PostSignInRedirectUri"));
            }

            return (Valid: true, ActionResult: null);
        }

        private (bool Valid, IActionResult? ActionResult) ValidatePostSignOutRedirectUri(SignOutRequest signOutRequest)
        {
            if (!string.IsNullOrEmpty(signOutRequest.PostSignOutRedirectUri) && signOutRequest.PostSignOutRedirectUri.EndsWith('/'))
            {
                // Trim trailing slash to ensure that redirect URIs are compared correctly in both the provider and api configuration.
                signOutRequest.PostSignOutRedirectUri = signOutRequest.PostSignOutRedirectUri.TrimEnd('/');
            }

            bool isAllowed = _options.AllowedPostSignOutRedirectUris.Any(allowed => string.Equals(allowed, signOutRequest.PostSignOutRedirectUri, StringComparison.OrdinalIgnoreCase));

            if (!isAllowed)
            {
                _logger.LogError(
                    "Invalid PostSignOutRedirectUri. {PostSignOutRedirectUri} does not exist in the list of allowed client redirect uri's: {PostSignOutRedirectUris}",
                    signOutRequest.PostSignOutRedirectUri,
                    string.Join(",", _options.AllowedPostSignOutRedirectUris)
                );

                return (Valid: false, ActionResult: BadRequest("Invalid PostSignOutRedirectUri"));
            }

            return (Valid: true, ActionResult: null);
        }
    }
}
