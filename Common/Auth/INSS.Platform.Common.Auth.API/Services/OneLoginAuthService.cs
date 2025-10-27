using Azure.Security.KeyVault.Secrets;
using INSS.Platform.Common.Auth.Contracts.Request;
using INSS.Platform.Common.Auth.Contracts.Response;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Specialized;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Web;

namespace INSS.Platform.Common.Auth.API.Services
{
    /// <summary>
    /// Provides authentication services for OneLogin integration.
    /// </summary>
    public class OneLoginAuthService : IAuthService
    {
        private const string HttpClientName = "AuthenticationClient";

        private readonly ILogger<OneLoginAuthService> _logger;
        private readonly IConfiguration _appConfig;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SecretClient _secretClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneLoginAuthService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="appConfig">The application configuration.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public OneLoginAuthService(
            ILogger<OneLoginAuthService> logger,
            IConfiguration appConfig,
            IHttpClientFactory httpClientFactory,
            SecretClient secretClient)
        {
            _logger = logger;
            _appConfig = appConfig;
            _httpClientFactory = httpClientFactory;
            this._secretClient = secretClient;
        }

        /// <inheritdoc/>
        public async Task<string> GetLoginRedirectUrlAsync(LoginRequest loginRequest)
        {
            GetOneLoginAuthorizeRequestConfig(out string clientId, out string authorizeUri, out string scopes, out string _);

            Dictionary<string, object> requestClaims = await BuildAuthorizeRequestClaimsAsync(loginRequest).ConfigureAwait(false);

            string queryPrivateKeyPem = await GetQueryJwtPrivateKeyAsync().ConfigureAwait(false);
            string securedRequest = CreateJwtSecurityTokenAsync(clientId, authorizeUri, queryPrivateKeyPem, requestClaims);

            return $"{authorizeUri}?response_type=code" +
                   $"&scope={Uri.EscapeDataString(scopes)}" +
                   $"&client_id={Uri.EscapeDataString(clientId)}" +
                   $"&request={Uri.EscapeDataString(securedRequest)}";
        }

        /// <inheritdoc/>
        public async Task<bool> LogoutAsync(LogoutRequest logoutRequest)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName);
            try
            {
                UriBuilder uriBuilder = new(_appConfig["OneLogin:LogoutUri"] ?? string.Empty);
                NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

                query["id_token_hint"] = logoutRequest.IdToken;
                uriBuilder.Query = query.ToString();

                HttpResponseMessage logoutResponse = await httpClient.GetAsync(uriBuilder.Uri).ConfigureAwait(false);

                if(logoutResponse.StatusCode == HttpStatusCode.Found) 
                {
                    return true;
                }

                string errorContent = await logoutResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                _logger.LogError("Logout endpoint returned error: {StatusCode} - {Content}", logoutResponse.StatusCode, errorContent);

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling user logout.");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<TokenData> HandleCallbackAsync(string authorizationCode, string nonce)
        {
            GetOneLoginTokenRequestConfig(out string clientId, out string tokenUri, out string redirectUri);

            HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName);
            try
            {
                FormUrlEncodedContent urlEncodedContent = new(await BuildTokenRequestParametersAsync(authorizationCode, tokenUri, redirectUri, clientId).ConfigureAwait(false));
                HttpResponseMessage tokenResponse = await httpClient.PostAsync(tokenUri, urlEncodedContent).ConfigureAwait(false);

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    string errorContent = await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError("Token endpoint returned error: {StatusCode} - {Content}", tokenResponse.StatusCode, errorContent);
                    throw new InvalidOperationException($"Token endpoint error: {tokenResponse.StatusCode}");
                }

                string tokenContent = await tokenResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                JsonDocument tokenJson = JsonDocument.Parse(tokenContent);
                string? accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
                string? idToken = tokenJson.RootElement.GetProperty("id_token").GetString();

                ValidateIdTokenClaims(idToken, nonce);

                return new TokenData
                {
                    AccessToken = accessToken ?? string.Empty,
                    IdToken = idToken ?? string.Empty,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling authentication callback.");
                return new ();
            }
        }

        /// <inheritdoc/>
        public async Task<(bool isValid, string nonce, string csrfToken, string userId, string clientUrl)> ValidateAndExtractRequestStateAsync(string token)
        {
            GetOneLoginStateValidationConfig(out string clientId, out string authorizeUri);

            string publicKeyPem = await GetStateJwtPublicKeyAsync().ConfigureAwait(false);

            RSA rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);

            RsaSecurityKey publicKey = new(rsa);

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = clientId,
                ValidateAudience = true,
                ValidAudience = authorizeUri,
                ValidateLifetime = true,
                IssuerSigningKey = publicKey,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            JwtSecurityTokenHandler handler = new();
            try
            {
                handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                IEnumerable<Claim> claims = GetClaimsFromJwt(token);
                string nonce = claims.FirstOrDefault(c => c.Type == "nonce")?.Value ?? string.Empty;
                string csrfToken = claims.FirstOrDefault(c => c.Type == "csrfToken")?.Value ?? string.Empty;
                string userId = claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? string.Empty;
                string clientUrl = claims.FirstOrDefault(c => c.Type == "clientUrl")?.Value ?? string.Empty;

                return (true, nonce, csrfToken, userId, clientUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request state validation failed on callback.");
                return (false, string.Empty, string.Empty, string.Empty, string.Empty);
            }
        }

        #region Jwt Creation & Claim Validation


        /// <summary>
        /// Creates a JWT security token for authentication flows.
        /// </summary>
        /// <param name="clientId">The client ID to use as the issuer of the token.</param>
        /// <param name="audienceEndpoint">The audience endpoint for which the token is intended.</param>
        /// <param name="privateKeyPem">The PEM-formatted RSA private key used to sign the token.</param>
        /// <param name="claims">A dictionary containing the claims to include in the token.</param>
        /// <returns>
        /// The serialized JWT token as a string.
        /// </returns>
        private static string CreateJwtSecurityTokenAsync(string clientId, string audienceEndpoint, string privateKeyPem, Dictionary<string, object> claims)
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
                expires: DateTime.UtcNow.AddMinutes(5),
                issuedAt: DateTime.UtcNow,
                signingCredentials: credentials,
                encryptingCredentials: null,
                claimCollection: claims
            );

            return handler.WriteToken(token);
        }

        /// <summary>
        /// Validates the claims in the provided ID token against the expected nonce and CSRF token.
        /// </summary>
        /// <param name="idToken">The ID token to validate.</param>
        /// <param name="expectedNonce">The expected nonce value.</param>
        /// <exception cref="SecurityTokenException">
        /// Thrown if the nonce or CSRF token claims are missing or do not match the expected values.
        /// </exception>
        private void ValidateIdTokenClaims(string? idToken, string expectedNonce)
        {
            IEnumerable<Claim> claims = GetClaimsFromJwt(idToken ?? string.Empty);

            Claim? nonceClaim = claims.FirstOrDefault(c => c.Type == "nonce");
            if (nonceClaim == null || nonceClaim.Value != expectedNonce)
            {
                _logger.LogError("ID token nonce claim is missing or does not match the expected nonce.");
                throw new SecurityTokenException("Invalid ID token nonce.");
            }
        }

        /// <summary>
        /// Extracts claims from a JWT token string.
        /// </summary>
        /// <param name="jwtToken">The JWT token as a string.</param>
        /// <returns>
        /// An enumerable collection of <see cref="Claim"/> objects extracted from the token.
        /// Returns an empty collection if the token is null, empty, or invalid.
        /// </returns>
        private IEnumerable<Claim> GetClaimsFromJwt(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
            {
                _logger.LogError("Error getting claims. JWT token is null or empty.");
                return [];
            }

            try
            {
                JwtSecurityTokenHandler handler = new();
                JwtSecurityToken token = handler.ReadJwtToken(jwtToken);
                return token.Claims;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error getting claims. Invalid JWT token format.");
                return [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting claims. Unexpected error while parsing JWT token.");
                return [];
            }
        }

        #endregion

        #region Claims & Parameter Builders

        /// <summary>
        /// Builds the claims dictionary for the authorize request JWT, used in OneLogin authentication flows.
        /// </summary>
        /// <param name="loginRequest">
        /// The login request containing the client URL, user ID, and CSRF token.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a dictionary of claims for the authorize request JWT.
        /// </returns>
        private async Task<Dictionary<string, object>> BuildAuthorizeRequestClaimsAsync(LoginRequest loginRequest)
        {
            const string responseType = "code";
            string nonce = Guid.NewGuid().ToString("N");
            string vtr = "[\"Cl.Cm\"]";
            string uiLocales = "en"; // English only for now but Welsh is supported by OneLogin as cy

            GetOneLoginAuthorizeRequestConfig(out string clientId, out string authorizeUri, out string scopes, out string redirectUri);

            Dictionary<string, object> stateClaims = BuildAuthorizeRequestStateClaims(loginRequest.ClientUrl, loginRequest.UserId, nonce, loginRequest.CsrfToken);
            string statePrivateKeyPem = await GetStateJwtPrivateKeyAsync().ConfigureAwait(false);
            string stateToken = CreateJwtSecurityTokenAsync(clientId, authorizeUri, statePrivateKeyPem, stateClaims);

            return new Dictionary<string, object>
            {
                { "response_type", responseType },
                { "scope", scopes },
                { "client_id", clientId },
                { "state", stateToken },
                { "redirect_uri", redirectUri },
                { "nonce", nonce },
                { "aud", authorizeUri },
                { "iss", clientId },
                { "ui_locales", uiLocales },
                { "vtr", vtr }
            };
        }

        /// <summary>
        /// Builds the parameters required for the token request to the OneLogin token endpoint.
        /// </summary>
        /// <param name="authorizationCode">The authorization code received from the authentication provider.</param>
        /// <param name="tokenUri">The URI of the token endpoint.</param>
        /// <param name="redirectUri">The redirect URI registered with the authentication provider.</param>
        /// <param name="clientId">The client ID used for authentication.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a dictionary of token request parameters.
        /// </returns>
        private async Task<Dictionary<string, string>> BuildTokenRequestParametersAsync(string authorizationCode, string tokenUri, string redirectUri, string clientId)
        {
            Dictionary<string, object> clientAssertionClaims = BuildClientAssertionClaims(clientId, tokenUri);
            string privateKeyPem = await GetQueryJwtPrivateKeyAsync().ConfigureAwait(false);
            string clientAssertionToken = CreateJwtSecurityTokenAsync(clientId, tokenUri, privateKeyPem, clientAssertionClaims);

            return new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                { "client_assertion", clientAssertionToken },
                { "code", authorizationCode },
                { "redirect_uri", redirectUri },
                { "grant_type", "authorization_code" }
            };
        }

        /// <summary>
        /// Builds the claims dictionary for the authorize request state JWT, used in OneLogin authentication flows.
        /// </summary>
        /// <param name="clientUrl">The client application's URL to redirect back after authentication.</param>
        /// <param name="userId">The unique identifier of the user initiating the login.</param>
        /// <param name="nonce">A unique nonce value for the authentication request.</param>
        /// <param name="csrfToken">A CSRF token to protect against cross-site request forgery.</param>
        /// <returns>
        /// A dictionary containing the claims for the request state JWT.
        /// </returns>
        private static Dictionary<string, object> BuildAuthorizeRequestStateClaims(string clientUrl, string userId, string nonce, string csrfToken)
        {
            return new Dictionary<string, object>
            {
                { "csrfToken", csrfToken },
                { "nonce", nonce },
                { "userId", userId},
                { "clientUrl", clientUrl},
                { "iat", EpochTime.GetIntDate(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture) },
                { "exp", EpochTime.GetIntDate(DateTime.UtcNow.AddMinutes(5)).ToString(CultureInfo.InvariantCulture) },
            };
        }

        /// <summary>
        /// Builds the claims dictionary for the client assertion JWT.
        /// </summary>
        /// <param name="clientId">The client ID to use as issuer and subject.</param>
        /// <param name="audienceEndpoint">The audience endpoint for the token.</param>
        /// <returns>
        /// A dictionary containing the claims for the client assertion.
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

        #endregion

        #region Key Retrieval

        /// <summary>
        /// Retrieves the private key used for signing query JWTs.
        /// Attempts to read the key from a configured file; if not found, retrieves it from Azure Key Vault.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the private key as a PEM-formatted string.
        /// </returns>
        protected async Task<string> GetQueryJwtPrivateKeyAsync()
        {
            string keyFileName = _appConfig["OneLogin:QueryJwtPrivateKeyFile"] ?? string.Empty;

            string keyFromFile = await GetKeyFromFileIfConfiguredAsync(keyFileName).ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(keyFromFile)
                ? keyFromFile
                : await GetKeyVaultSecretAsync("QueryJwtPrivateKey").ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the private key used for signing state JWTs.
        /// Attempts to read the key from a configured file; if not found, retrieves it from Azure Key Vault.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the private key as a PEM-formatted string.
        /// </returns>
        protected async Task<string> GetStateJwtPrivateKeyAsync()
        {
            string keyFileName = _appConfig["OneLogin:StateJwtPrivateKeyFile"] ?? string.Empty;

            string keyFromFile = await GetKeyFromFileIfConfiguredAsync(keyFileName).ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(keyFromFile)
                ? keyFromFile
                : await GetKeyVaultSecretAsync("StateJwtPrivateKey").ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the public key used for validating state JWTs.
        /// Attempts to read the key from a configured file; if not found, retrieves it from Azure Key Vault.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the public key as a PEM-formatted string.
        /// </returns>
        protected async Task<string> GetStateJwtPublicKeyAsync()
        {
            string keyFileName = _appConfig["OneLogin:StateJwtPublicKeyFile"] ?? string.Empty;

            string keyFromFile = await GetKeyFromFileIfConfiguredAsync(keyFileName).ConfigureAwait(false);

            return !string.IsNullOrWhiteSpace(keyFromFile) 
                ? keyFromFile 
                : await GetKeyVaultSecretAsync("StateJwtPublicKey").ConfigureAwait(false);
        }

        /// <summary>
        /// Attempts to read a key from a file if a file name is configured.
        /// </summary>
        /// <param name="keyFileName">The name of the key file.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the key as a string, or an empty string if not found.
        /// </returns>
        private static async Task<string> GetKeyFromFileIfConfiguredAsync(string keyFileName)
        {
            if (!string.IsNullOrWhiteSpace(keyFileName))
            {
                string keyFilePath = Path.Combine(AppContext.BaseDirectory, keyFileName);
                if (File.Exists(keyFilePath))
                {
                    return await File.ReadAllTextAsync(keyFilePath).ConfigureAwait(false);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrieves a secret value from Azure Key Vault.
        /// </summary>
        /// <param name="secretName">The name of the secret to retrieve.</param>
        /// <returns>The secret value as a string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the KeyVault URI is missing.</exception>
        private async Task<string> GetKeyVaultSecretAsync(string secretName)
        {
            try
            {
                KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName).ConfigureAwait(false);
                _logger.LogInformation("Successfully retrieved secret: {SecretName}", secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving secret: {SecretName}", secretName);
                throw;
            }
        }

        #endregion

        #region Configuration Retrieval

        /// <summary>
        /// Retrieves OneLogin configuration values required for the authorize request.
        /// </summary>
        /// <param name="clientId">Outputs the OneLogin client ID.</param>
        /// <param name="authorizeUri">Outputs the OneLogin authorize URI.</param>
        /// <param name="scopes">Outputs the OneLogin scopes.</param>
        /// <param name="redirectUri">Outputs the OneLogin redirect URI.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if any required configuration value is missing.
        /// </exception>
        private void GetOneLoginAuthorizeRequestConfig(out string clientId, out string authorizeUri, out string scopes, out string redirectUri)
        {
            clientId = _appConfig["OneLogin:ClientId"] ?? string.Empty;
            authorizeUri = _appConfig["OneLogin:AuthorizeUri"] ?? string.Empty;
            scopes = _appConfig["OneLogin:Scopes"] ?? string.Empty;
            redirectUri = _appConfig["OneLogin:RedirectUri"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(clientId) ||
                    string.IsNullOrWhiteSpace(authorizeUri) ||
                    string.IsNullOrWhiteSpace(scopes) ||
                    string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError("OneLogin configuration is incomplete in appsettings, ClientId, AuthorizeUri, Scopes or RedirectUri values are missing.");
                throw new InvalidOperationException("OneLogin configuration is missing.");
            }
        }

        /// <summary>
        /// Retrieves OneLogin configuration values required for the token request.
        /// </summary>
        /// <param name="clientId">Outputs the OneLogin client ID.</param>
        /// <param name="tokenUri">Outputs the OneLogin token URI.</param>
        /// <param name="redirectUri">Outputs the OneLogin redirect URI.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if any required configuration value is missing.
        /// </exception>
        private void GetOneLoginTokenRequestConfig(out string clientId, out string tokenUri, out string redirectUri)
        {
            clientId = _appConfig["OneLogin:ClientId"] ?? string.Empty;
            tokenUri = _appConfig["OneLogin:TokenUri"] ?? string.Empty;
            redirectUri = _appConfig["OneLogin:RedirectUri"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(tokenUri) ||
                string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError("OneLogin configuration is incomplete in appsettings, ClientId, TokenUri or RedirectUri values are missing.");
                throw new InvalidOperationException("OneLogin configuration is missing.");
            }
        }

        /// <summary>
        /// Retrieves OneLogin configuration values required for state validation.
        /// </summary>
        /// <param name="clientId">Outputs the OneLogin client ID.</param>
        /// <param name="authorizeUri">Outputs the OneLogin authorize URI.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if any required configuration value is missing.
        /// </exception>
        private void GetOneLoginStateValidationConfig(out string clientId, out string authorizeUri)
        {
            clientId = _appConfig["OneLogin:ClientId"] ?? string.Empty;
            authorizeUri = _appConfig["OneLogin:AuthorizeUri"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(authorizeUri))
            {
                _logger.LogError("OneLogin configuration is incomplete in appsettings, ClientId or AuthorizeUri values are missing.");
                throw new InvalidOperationException("OneLogin configuration is missing.");
            }
        }

        #endregion
    }
}
