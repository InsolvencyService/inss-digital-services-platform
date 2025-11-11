using INSS.Platform.UserManagement.Web.Models;
using INSS.Platform.UserManagement.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Specialized;
using System.Security.Claims;

namespace INSS.Platform.UserManagement.Web.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account authentication actions.
    /// </summary>
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AuthOptions _authenticationOptions;
        private readonly IJwtAuthenticationService _jwtAuthentication;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="authenticationOptions">The authentication options.</param>
        /// <param name="jwtAuthentication">The JWT authentication service.</param>
        public AccountController(ILogger<AccountController> logger, IOptions<AuthOptions> authenticationOptions, IJwtAuthenticationService jwtAuthentication)
        {
            _logger = logger;
            _authenticationOptions = authenticationOptions.Value;
            _jwtAuthentication = jwtAuthentication;
        }

        /// <summary>
        /// Handles the sign-in or sign-out callback, signs in the user if the token is valid. Redirects accordingly.
        /// </summary>
        /// <param name="token">
        /// The JWT token received from the authentication provider. If valid, the user will be signed in.
        /// If a JWT is not provided or is invalid, the user will be redirected to the home page.
        /// </param>
        /// <param name="redirectUrl">
        /// The optional URL to redirect to after successful sign-in. If not provided, defaults to the home page.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> that redirects the user to the specified <paramref name="redirectUrl"/> if provided and authentication is successful;
        /// otherwise, redirects to the home page.
        /// </returns>
        public async Task<IActionResult> Index(string? token, string? redirectUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(token) && _jwtAuthentication.ValidateJwt(token, out ClaimsPrincipal principal))
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Initiates the login process by redirecting to the authentication provider.
        /// </summary>
        /// <param name="returnUrl">The URL to redirect to after successful login.</param>
        /// <returns>An <see cref="IActionResult"/> that redirects to the authentication provider's sign-in endpoint.</returns>
        public IActionResult Login(string? returnUrl = null)
        {
            _logger.LogInformation("User initiated login.");

            string signInUrl = $"{_authenticationOptions.BaseApiUrl}/authentication/{_authenticationOptions.AuthProvider}/signin";
            string redirectUrl = BuildRedirectUrl(returnUrl);

            return Redirect($"{signInUrl}?clientRedirectUrl={redirectUrl}");
        }

        /// <summary>
        /// Signs out the current user and redirects to the authentication provider's sign-out endpoint.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that redirects to the sign-out endpoint.</returns>
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User initiated logout.");

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            string signOutUrl = $"{_authenticationOptions.BaseApiUrl}/authentication/{_authenticationOptions.AuthProvider}/signout";

            return Redirect(signOutUrl);
        }

        /// <summary>
        /// Builds the redirect URL for authentication callbacks, optionally including a return URL as a query parameter.
        /// </summary>
        /// <param name="returnUrl">
        /// The URL to redirect to after authentication. If relative, it is converted to an absolute URL.
        /// </param>
        /// <returns>
        /// An escaped redirect URI string suitable for use as a query parameter.
        /// </returns>
        /// <remarks>
        /// If a user was trying to access a protected resource before authentication, the requested returnUrl is appended as a query parameter to the base redirect URL.
        /// </remarks>
        private string BuildRedirectUrl(string? returnUrl)
        {
            string baseRedirectUrl = Url.Action("Index", "Account", null, Url.ActionContext.HttpContext.Request.Scheme)!;

            if (!string.IsNullOrEmpty(returnUrl))
            {
                string absoluteRedirectUrl = returnUrl;
                if (Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
                {
                    absoluteRedirectUrl = Url.ActionContext.HttpContext.Request.Scheme + "://" +
                                        Url.ActionContext.HttpContext.Request.Host +
                                        returnUrl;
                }

                UriBuilder uriBuilder = new(baseRedirectUrl);
                NameValueCollection query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                query["redirectUrl"] = absoluteRedirectUrl;
                uriBuilder.Query = query.ToString();

                return Uri.EscapeDataString(uriBuilder.ToString());
            }

            return Uri.EscapeDataString(baseRedirectUrl);
        }
    }
}
