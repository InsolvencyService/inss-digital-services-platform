using INSS.Platform.Auth.API.Models;
using INSS.Platform.Auth.Application;
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

namespace INSS.Platform.Auth.API.Services;

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
    private const string ReturnUrlKey = "returnUrl";
    private const string UserIdKey = "userId";

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
    public void HandleAuthorizationCodeReceived(
        AuthorizationCodeReceivedContext context, 
        AuthenticationProvider provider)
    {
        _logger.LogInformation("Processing AuthorizationCodeReceived event for {Provider}.", provider.ToString());

        const int JwtExpiryMinutes = 5;
        string clientId = _options.OneLogin.ClientId;
        string audienceEndpoint = _options.OneLogin.TokenUri;

        Dictionary<string, object> clientAssertionClaims = BuildClientAssertionClaims(clientId, audienceEndpoint);
        string clientAssertionToken = CreateJwtSecurityToken(clientId, audienceEndpoint, _options.OneLogin.JwtPrivateKey, JwtExpiryMinutes, clientAssertionClaims);

        if (context.TokenEndpointRequest is not null)
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

        if (!ValidateAndStoreTokens(context, out string accessToken))
        {
            return;
        }

        await context.HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            context.Principal!,
            context.Properties).ConfigureAwait(false);

        _logger.LogInformation("User {User} successfully authenticated via {Provider}.", context.Principal?.Identity?.Name, provider.ToString());

        UriBuilder uriBuilder = BuildRedirectUrlWithToken(context, accessToken);
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

        AuthenticateResult authenticateResult = await context.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
        string idToken = authenticateResult.Properties?.GetTokenValue("id_token") ?? string.Empty;
        context.ProtocolMessage.IdTokenHint = idToken;

        // After the user signs out from the identity provider, redirect them back to this api to complete the sign-out process.
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
    /// Creates a JWT security token with the specified issuer, audience, private key, expiry, and claims.
    /// </summary>
    /// <param name="issuer">The issuer of the JWT token.</param>
    /// <param name="audienceEndpoint">The audience endpoint (intended recipient) of the JWT token.</param>
    /// <param name="privateKeyPem">The private key in PEM format used to sign the JWT token.</param>
    /// <param name="expiryMinutes">The expiry duration (in minutes) for the JWT token.</param>
    /// <param name="claims">A dictionary of claims to include in the JWT token.</param>
    /// <returns>
    /// A signed JWT token as a string.
    /// </returns>
    private static string CreateJwtSecurityToken(string issuer, string audienceEndpoint, string privateKeyPem, int expiryMinutes, Dictionary<string, object> claims)
    {
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);

        RsaSecurityKey privateKey = new(rsa);
        SigningCredentials credentials = new(privateKey, SecurityAlgorithms.RsaSha256);

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.CreateJwtSecurityToken(
            issuer: issuer,
            audience: audienceEndpoint,
            subject: null,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            issuedAt: DateTime.UtcNow,
            signingCredentials: credentials,
            encryptingCredentials: null,
            claimCollection: claims
        );

        return handler.WriteToken(token);
    }

    /// <summary>
    /// Creates an INSS JWT by extracting the 'sub' claim from the provider-issued access token and optionally including a user ID.
    /// </summary>
    /// <param name="providerIssuedAccessToken">The access token issued by the external authentication provider. This token must contain a 'sub' claim.</param>
    /// <param name="userid">The optional user ID to include in the generated JWT as the 'user_id' claim.</param>
    /// <returns>
    /// A signed JWT string containing the 'sub' claim (and 'user_id' if provided), issued by the INSS platform.
    /// </returns>
    private string CreateInssJwtSecurityToken(string providerIssuedAccessToken, string? userid)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken providerToken = handler.ReadJwtToken(providerIssuedAccessToken);

        Dictionary<string, object> claims = new();
        Claim? subClaim = providerToken.Claims.FirstOrDefault(claim => claim.Type == "sub");
        if (subClaim != null)
        {
            claims["sub"] = subClaim.Value;
        }

        if (!string.IsNullOrEmpty(userid))
        {
            claims["user_id"] = userid;
        }

        return CreateJwtSecurityToken(
            _options.Inss.Issuer,
            _options.Inss.Audience,
            _options.Inss.JwtPrivateKey,
            _options.Inss.ExpiryMinutes,
            claims
        );
    }

    /// <summary>
    /// Validates the presence of required tokens in the <see cref="TokenValidatedContext"/> and stores them in the authentication properties.
    /// </summary>
    /// <param name="context">The context containing information about the token validation event.</param>
    /// <param name="accessToken">When this method returns, contains the access token if validation is successful; otherwise, an empty string.</param>
    /// <returns>
    /// <c>true</c> if the tokens are valid and successfully stored; otherwise, <c>false</c>.
    /// </returns>
    private static bool ValidateAndStoreTokens(TokenValidatedContext context, out string accessToken)
    {
        accessToken = string.Empty;

        if (context.Properties is null)
        {
            context.Fail("TokenValidatedContext.Properties is null.");
            return false;
        }

        if (context.Principal is null)
        {
            context.Fail("TokenValidatedContext.Principal is null.");
            return false;
        }

        accessToken = context.TokenEndpointResponse?.AccessToken ?? string.Empty;
        string idToken = context.TokenEndpointResponse?.IdToken ?? string.Empty;

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(idToken))
        {
            context.Fail($"Failure accessing required authentication parameters. access_token:{accessToken}, id_token:{idToken}");
            return false;
        }

        context.Properties.StoreTokens(
        [
            new AuthenticationToken { Name = "id_token", Value = idToken },
            new AuthenticationToken { Name = "access_token", Value = accessToken },
        ]);

        return true;
    }

    /// <summary>
    /// Builds a redirect URL with the generated INSS JWT token appended as a query parameter.
    /// </summary>
    /// <param name="context">The context containing information about the token validation event.</param>
    /// <param name="accessToken">The access token issued by the external authentication provider.</param>
    /// <returns>
    /// A <see cref="UriBuilder"/> representing the redirect URL with the INSS JWT token included as a query parameter.
    /// </returns>
    private UriBuilder BuildRedirectUrlWithToken(TokenValidatedContext context, string accessToken)
    {
        string? userId = null;
        context.Properties?.Items.TryGetValue(UserIdKey, out userId);

        string? returnUrl = null;
        context.Properties?.Items.TryGetValue(ReturnUrlKey, out returnUrl);

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = context.Request.Headers.Referer;
        }

        string inssJwt = CreateInssJwtSecurityToken(accessToken, userId);

        UriBuilder uriBuilder = new(returnUrl!);
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["token"] = inssJwt;
        uriBuilder.Query = query.ToString();
        return uriBuilder;
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