using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using INSS.Platform.Auth.API.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text.Json;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Provides authentication services for OneLogin integration.
    /// </summary>
    public class OneLoginAuthService : IAuthService
    {
        private readonly ILogger<OneLoginAuthService> _logger;
        private readonly IConfiguration _appConfig;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneLoginAuthService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="appConfig">The application configuration.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public OneLoginAuthService(
            ILogger<OneLoginAuthService> logger,
            IConfiguration appConfig,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _appConfig = appConfig;
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc/>
        public string GetLoginRedirectUrl(string stateCacheKey)
        {
            string clientId = _appConfig["OneLogin:ClientId"] ?? string.Empty;
            string baseUrl = _appConfig["OneLogin:SignInUri"] ?? string.Empty;
            string scopes = _appConfig["OneLogin:Scopes"] ?? string.Empty;
            string redirectUri = _appConfig["OneLogin:RedirectUri"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(clientId) ||
                    string.IsNullOrWhiteSpace(baseUrl) ||
                    string.IsNullOrWhiteSpace(scopes) ||
                    string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError("OneLogin configuration is incomplete in appsettings.");
                throw new InvalidOperationException("OneLogin configuration is missing.");
            }

            string responseType = "code";
            string nonce = "aEwkamaos5B";
            string vtr = "[\"Cl.Cm\"]"; // Medium authentication
            string uiLocales = "en";

            Dictionary<string, string> queryParams = new ()
            {
                { "client_id", clientId },
                { "redirect_uri", redirectUri },
                { "response_type", responseType },
                { "scope", scopes },
                { "state", stateCacheKey },
                { "nonce", nonce },
                { "vtr", vtr },
                { "ui_locales", uiLocales }
            };

            string queryString = string.Join("&", queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            return $"{baseUrl}?{queryString}";
        }

        /// <inheritdoc/>
        public async Task<TokenData> HandleCallbackAsync(string authorizationCode)
        {
            string clientId = _appConfig["OneLogin:ClientId"] ?? string.Empty;
            string tokenUri = _appConfig["OneLogin:TokenUri"] ?? string.Empty;
            string redirectUri = _appConfig["OneLogin:RedirectUri"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(tokenUri) ||
                string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError("OneLogin configuration is incomplete in appsettings, ClientId or TokenUri values are missing.");
                throw new InvalidOperationException("OneLogin configuration is missing.");
            }

            HttpClient httpClient = _httpClientFactory.CreateClient();
            try
            {
                string clientAssertion = await GenerateClientAssertionAsync(clientId, tokenUri);

                HttpResponseMessage tokenResponse = await httpClient.PostAsync(tokenUri,
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", clientId },
                        { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                        { "client_assertion", clientAssertion },
                        { "code", authorizationCode },
                        { "redirect_uri", redirectUri },
                        { "grant_type", "authorization_code" }
                    }));

                if (!tokenResponse.IsSuccessStatusCode)
                {
                    string errorContent = await tokenResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Token endpoint returned error: {StatusCode} - {Content}", tokenResponse.StatusCode, errorContent);
                    throw new InvalidOperationException($"Token endpoint error: {tokenResponse.StatusCode}");
                }

                string tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                JsonDocument tokenJson = JsonDocument.Parse(tokenContent);
                string? accessToken = tokenJson.RootElement.GetProperty("access_token").GetString();
                string? idToken = tokenJson.RootElement.GetProperty("id_token").GetString();

                return new TokenData
                {
                    AccessToken = accessToken ?? string.Empty,
                    IdToken = idToken ?? string.Empty,
                    ExpiresIn = tokenJson.RootElement.GetProperty("expires_in").GetInt32()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling authentication callback.");
                throw;
            }
        }

        /// <inheritdoc/>
        public Task<bool> IsAuthenticatedAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task LogOutAsync(string idToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates a client assertion JWT for authenticating with the OneLogin token endpoint.
        /// </summary>
        /// <param name="clientId">The client ID for OneLogin.</param>
        /// <param name="tokenEndpoint">The token endpoint URI.</param>
        /// <returns>A JWT string for client assertion.</returns>
        private async Task<string> GenerateClientAssertionAsync(string clientId, string tokenEndpoint)
        {
            string pemKey = await GetClientAssertionKey();
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(pemKey);

            RsaSecurityKey securityKey = new (rsa);
            SigningCredentials credentials = new (securityKey, SecurityAlgorithms.RsaSha256);

            JwtSecurityTokenHandler handler = new ();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(
                issuer: clientId,
                audience: tokenEndpoint,
                subject: new System.Security.Claims.ClaimsIdentity(),
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(5),
                issuedAt: DateTime.UtcNow,
                signingCredentials: credentials,
                encryptingCredentials: null, // Fix: add this argument for required parameter
                claimCollection: new Dictionary<string, object>
                {
                    { "sub", clientId },
                    { "jti", Guid.NewGuid().ToString() }
                }
            );

            return handler.WriteToken(token);
        }

        /// <summary>
        /// Retrieves the client assertion key from a file or Azure Key Vault.
        /// </summary>
        /// <returns>The PEM-encoded client assertion key.</returns>
        private async Task<string> GetClientAssertionKey()
        {
            string? keyFileName = _appConfig["OneLogin:ClientAssertionKeyFile"];
            if (!string.IsNullOrWhiteSpace(keyFileName))
            {
                string keyFilePath = Path.Combine(AppContext.BaseDirectory, keyFileName);
                if (File.Exists(keyFilePath))
                {
                    return await File.ReadAllTextAsync(keyFilePath).ConfigureAwait(false);
                }
            }

            return await GetKeyVaultSecretAsync("ClientAssertionKey").ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a secret value from Azure Key Vault.
        /// </summary>
        /// <param name="secretName">The name of the secret to retrieve.</param>
        /// <returns>The secret value as a string.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the KeyVault URI is missing.</exception>
        private async Task<string> GetKeyVaultSecretAsync(string secretName)
        {
            string keyVaultUriString = _appConfig["KeyVault:Uri"] ?? string.Empty;
            if (string.IsNullOrWhiteSpace(keyVaultUriString))
            {
                _logger.LogError("KeyVault URI is not configured in appsettings.");
                throw new InvalidOperationException("KeyVault URI is missing.");
            }

            Uri keyVaultUri = new(keyVaultUriString);
            SecretClient client = new(keyVaultUri, new DefaultAzureCredential());

            try
            {
                KeyVaultSecret secret = await client.GetSecretAsync(secretName);
                _logger.LogInformation("Successfully retrieved secret: {SecretName}", secretName);
                return secret.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving secret: {SecretName}", secretName);
                throw;
            }
        }
    }
}
