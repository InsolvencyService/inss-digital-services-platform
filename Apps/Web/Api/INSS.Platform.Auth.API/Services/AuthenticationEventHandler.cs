using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Web;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Handles authentication-related events for external identity providers, including processing authorization codes,
    /// validating tokens, handling sign-out redirection, and managing remote authentication failures.
    /// </summary>
    /// <remarks>This class implements event handlers for authentication workflows involving external
    /// providers such as OneLogin. It is typically used to customize the authentication process, including generating
    /// client assertion JWTs, managing token storage, and handling error responses. The event handlers are invoked by
    /// the authentication middleware during various stages of the authentication lifecycle.</remarks>
    public class AuthenticationEventHandler : IAuthenticationEventHandler
    {
        private readonly ILogger<AuthenticationEventHandler> _logger;
        private readonly AuthenticationProviderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationEventHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger instance used for logging authentication events.</param>
        /// <param name="options">The authentication provider options.</param>
        public AuthenticationEventHandler(
            ILogger<AuthenticationEventHandler> logger,
            IOptions<AuthenticationProviderOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }


        /// <inheritdoc/>
        public async Task HandleAuthorizationCodeReceivedAsync(
            AuthorizationCodeReceivedContext context, 
            AuthenticationProvider provider)
        {
            _logger.LogInformation("Processing AuthorizationCodeReceived event for {Provider}.", provider.ToString());

            string clientId = _options.OneLogin.ClientId;
            string audienceEndpoint = _options.OneLogin.TokenUri;

            Dictionary<string, object> clientAssertionClaims = BuildClientAssertionClaims(clientId, audienceEndpoint);
            string clientAssertionToken = CreateJwtSecurityToken(clientId, audienceEndpoint, _options.OneLogin.JwtPrivateKey, clientAssertionClaims);

            if (context.TokenEndpointRequest != null)
            {
                context.TokenEndpointRequest.ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                context.TokenEndpointRequest.ClientAssertion = clientAssertionToken;
            }
        }

        /// <inheritdoc/>
        public async Task HandleTokenValidatedAsync(
            TokenValidatedContext context,
            AuthenticationProvider provider)
        {
            _logger.LogInformation("Processing TokenValidated event for {Provider}.", provider.ToString());

            if (context.Properties is null)
            {
                _logger.LogError("Authentication properties are missing in TokenValidatedContext.");
                context.Fail("Authentication properties are missing.");
                return;
            }

            if (!context.Properties.Items.TryGetValue("returnUrl", out string? returnUrl))
            {
                returnUrl = context.Request.Headers.Referer;
            }

            string accessToken = context.TokenEndpointResponse?.AccessToken ?? string.Empty;
            string idToken = context.TokenEndpointResponse?.IdToken ?? string.Empty;

            if (string.IsNullOrEmpty(returnUrl) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(idToken))
            {
                _logger.LogError("Failure accessing required authentication parameters. returnUrl:{ReturnUrl}, access_token:{AccessToken}, id_token:{IdToken}", returnUrl, accessToken, idToken);
                context.Fail($"Failure accessing required authentication paramaters. returnUrl:{returnUrl}, access_token:{accessToken}, id_token:{idToken}");
                return;
            }

            context.Properties.StoreTokens(
            [
                new AuthenticationToken { Name = "id_token", Value = idToken },
                new AuthenticationToken { Name = "access_token", Value = accessToken },
            ]);

            UriBuilder uriBuilder = new(returnUrl);
            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            context.Properties.Items.TryGetValue("userId", out string? userId);
            if (!string.IsNullOrEmpty(userId))
            {
                query["user_id"] = userId;
            }

            query["access_token"] = accessToken;
            query["id_token"] = idToken;

            uriBuilder.Query = query.ToString();

            if (context.Principal == null)
            {
                _logger.LogError("Principal is missing after token validation.");
                context.Fail("Principal is missing after token validation.");
                return;
            }

            await context.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                context.Principal,
                context.Properties).ConfigureAwait(false);

            _logger.LogInformation("User {User} successfully authenticated via {Provider}.", context.Principal.Identity?.Name, provider.ToString());
            context.Response.Redirect(uriBuilder.Uri.ToString());
            context.HandleResponse();
        }

        /// <inheritdoc/>
        public async Task HandleRedirectToIdentityProviderForSignOutAsync(
            RedirectContext context,
            string signOutCallbackPath,
            AuthenticationProvider provider)
        {
            _logger.LogInformation("Processing RedirectToIdentityProviderForSignOut event for {Provider}.", provider.ToString());

            AuthenticateResult authenticateResult = await context.HttpContext.AuthenticateAsync("Cookies").ConfigureAwait(false);
            string idToken = authenticateResult.Properties?.GetTokenValue("id_token") ?? string.Empty;
            context.ProtocolMessage.IdTokenHint = idToken;

            string postLogoutRedirectUri = $"{context.Request.Scheme}://{context.Request.Host}/{signOutCallbackPath}";
            _logger.LogInformation("Setting PostLogoutRedirectUri to {PostLogoutRedirectUri}", postLogoutRedirectUri);
            context.ProtocolMessage.PostLogoutRedirectUri = postLogoutRedirectUri;
        }

        /// <inheritdoc/>
        public async Task HandleRemoteFailureAsync(
            RemoteFailureContext context,
            AuthenticationProvider provider)
        {
            _logger.LogInformation("Processing RemoteFailure event for {Provider}.", provider.ToString());

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var error = new { error = "Authentication failed", message = context.Failure?.Message ?? "Unknown error" };
            _logger.LogError("Remote authentication failure: {ErrorMessage}", error);

            await context.Response.WriteAsJsonAsync(error).ConfigureAwait(false);

            context.HandleResponse();
        }

        /// <summary>
        /// Creates a JWT security token for client assertion using the provided client ID, audience endpoint, private key, and claims.
        /// </summary>
        /// <param name="clientId">The client identifier to use as the issuer of the JWT.</param>
        /// <param name="audienceEndpoint">The audience endpoint (token endpoint URI) for which the JWT is intended.</param>
        /// <param name="privateKeyPem">The PEM-formatted RSA private key used to sign the JWT.</param>
        /// <param name="claims">A dictionary containing the claims to include in the JWT payload.</param>
        /// <returns>
        /// A string representation of the signed JWT security token.
        /// </returns>
        private static string CreateJwtSecurityToken(string clientId, string audienceEndpoint, string privateKeyPem, Dictionary<string, object> claims)
        {
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);

            RsaSecurityKey privateKey = new(rsa);
            SigningCredentials credentials = new(privateKey, SecurityAlgorithms.RsaSha256);

            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(
                issuer: clientId,
                audience: audienceEndpoint,
                subject: new ClaimsIdentity(),
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(60),
                issuedAt: DateTime.UtcNow,
                signingCredentials: credentials,
                encryptingCredentials: null,
                claimCollection: claims
            );

            return handler.WriteToken(token);
        }

        /// <summary>
        /// Builds the client assertion claims required for the JWT client assertion when authenticating with OneLogin.
        /// </summary>
        /// <param name="clientId">The client identifier (issuer and subject of the JWT).</param>
        /// <param name="audienceEndpoint">The audience endpoint (intended recipient of the JWT).</param>
        /// <returns>
        /// A dictionary containing the standard JWT claims for client assertion:
        /// <list type="bullet">
        /// <item><description><c>aud</c>: The audience (token endpoint URI).</description></item>
        /// <item><description><c>iss</c>: The issuer (client ID).</description></item>
        /// <item><description><c>sub</c>: The subject (client ID).</description></item>
        /// <item><description><c>jti</c>: A unique identifier for the JWT.</description></item>
        /// <item><description><c>iat</c>: Issued at time (as epoch seconds).</description></item>
        /// <item><description><c>exp</c>: Expiration time (as epoch seconds, 60 minutes after <c>iat</c>).</description></item>
        /// </list>
        /// </returns>
        private static Dictionary<string, object> BuildClientAssertionClaims(string clientId, string audienceEndpoint)
        {
            return new Dictionary<string, object>
            {
                { "aud", audienceEndpoint },
                { "iss", clientId },
                { "sub", clientId },
                { "jti", Guid.NewGuid().ToString() },
                { "iat", EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture) },
                { "exp", EpochTime.GetIntDate(DateTime.UtcNow.AddMinutes(5)).ToString(CultureInfo.InvariantCulture) },
            };
        }
    }
}