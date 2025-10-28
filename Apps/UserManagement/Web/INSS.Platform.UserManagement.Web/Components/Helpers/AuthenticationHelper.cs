using INSS.Platform.Common.Auth.Contracts.Request;
using INSS.Platform.Common.Auth.Contracts.Response;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace INSS.Platform.UserManagement.Web.Components.Helpers
{
    public class AuthenticationHelper : IAuthenticationHelper
    {
        private const string HttpClientName = "AuthenticationClient";

        private readonly ILogger<AuthenticationHelper> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public AuthenticationHelper(
            ILogger<AuthenticationHelper> logger,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<string> GetLoginUrlAsync(LoginRequest loginRequest)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName);
            try
            {
                HttpResponseMessage loginResponse = await httpClient.PostAsJsonAsync("/authentication/login-url", loginRequest).ConfigureAwait(false);

                if (!loginResponse.IsSuccessStatusCode)
                {
                    string errorContent = await loginResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError("Error getting login url from authentication api: {ErrorContent}", errorContent);
                    return string.Empty;
                }

                return await loginResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting login url from authentication api.");
                return string.Empty;
            }
        }

        /// <inheritdoc />
        public async Task<string> LoginAsync(LoginRequest loginRequest)
        {
            string redirectUrl = string.Empty;
            HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName);
            try
            {
                HttpResponseMessage loginResponse = await httpClient.PostAsJsonAsync("/authentication/login", loginRequest).ConfigureAwait(false);

                if (loginResponse.StatusCode == System.Net.HttpStatusCode.Redirect)
                {
                    if (loginResponse.Headers.Location != null)
                    {
                        redirectUrl = loginResponse.Headers.Location.ToString()!;
                    }
                }

                if (!loginResponse.IsSuccessStatusCode)
                {
                    string errorContent = await loginResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError("Error calling login from authentication api: {ErrorContent}", errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting login url from authentication api.");
            }

            return redirectUrl;
        }

        public bool IsUserSignedIn()
        {
            return TryGetAuthTokenDataFromCookie(out TokenData? tokenData) 
                && !string.IsNullOrWhiteSpace(tokenData?.AccessToken) 
                && !IsJwtExpired(tokenData.AccessToken);
        }

        public bool JwtExistsButHasExpired()
        {
            return TryGetAuthTokenDataFromCookie(out TokenData? tokenData) 
                && !string.IsNullOrWhiteSpace(tokenData?.AccessToken) 
                && IsJwtExpired(tokenData.AccessToken);
        }

        /// <inheritdoc />
        public async Task<bool> LogoutAsync(LogoutRequest logoutRequest)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName);
            try
            {
                HttpResponseMessage logoutResponse = await httpClient.PostAsJsonAsync("/authentication/logout", logoutRequest).ConfigureAwait(false);

                if (!logoutResponse.IsSuccessStatusCode)
                {
                    string errorContent = await logoutResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _logger.LogError("Error calling login from authentication api: {ErrorContent}", errorContent);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting login url from authentication api.");
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public void PersistAuthTokenDataInCookie(Uri rootUri)
        {
            Dictionary<string, StringValues> query = QueryHelpers.ParseQuery(rootUri.Query);

            if (!QueryHelper.TryGetQueryValue(query, "access_token", out string? accessToken)
                || !QueryHelper.TryGetQueryValue(query, "id_token", out string? idToken))
            {
                _logger.LogError("One or more required token parameters are missing in the URL.");
                throw new ArgumentException("One or more required token parameters are missing in the URL.");
            }

            TokenData token = new()
            {
                AccessToken = accessToken!,
                IdToken = idToken!,
            };

            CookieOptions options = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                // Set a long expiry so the cookie persists, actual token expiry is managed via JWT 
                // This allows us to detect if the user has logged in but the token is expired so we can force a re-login
                Expires = DateTimeOffset.UtcNow.AddDays(1), 
                MaxAge = TimeSpan.FromDays(1)               

            };

            string cookieName = _configuration["Auth:AuthCookieName"]!;
            string json = JsonSerializer.Serialize(token, _jsonSerializerOptions);
            HttpContext httpContext = _httpContextAccessor.HttpContext!;
            httpContext.Response.Cookies.Append(cookieName, json, options);
        }

        /// <inheritdoc />
        public bool TryGetAuthTokenDataFromCookie(out TokenData? tokenData)
        {
            tokenData = null;
            HttpContext httpContext = _httpContextAccessor.HttpContext!;
            if (httpContext == null)
            {
                return false;
            }

            string cookieName = _configuration["Auth:AuthCookieName"]!;
            if (!httpContext.Request.Cookies.TryGetValue(cookieName, out string? cookieValue)
                || string.IsNullOrWhiteSpace(cookieValue))
            {
                return false;
            }

            try
            {
                tokenData = JsonSerializer.Deserialize<TokenData>(cookieValue, _jsonSerializerOptions);
                return tokenData != null;
            }
            catch(JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing token data from cookie.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deserializing token data from cookie.");
                return false;
            }
        }

        /// <inheritdoc />
        public string CreateAndPersistCsrfTokenInCookie()
        {
            CookieOptions options = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(1)
            };

            string csrfToken = Guid.NewGuid().ToString("N");
            string cookieName = _configuration["Auth:CsrfCookieName"]!;
            HttpContext httpContext = _httpContextAccessor.HttpContext!;
            httpContext.Response.Cookies.Append(cookieName, csrfToken, options);

            return csrfToken;
        }

        /// <inheritdoc />
        public bool ValidateCsrfTokenInCookie(string csrfToken)
        {
            return TryGetCsrfTokenFromCookie(out string? storedCsrfToken) 
                && string.Equals(csrfToken, storedCsrfToken, StringComparison.Ordinal);
        }

        /// <inheritdoc />
        public string? GetLoginProviderUserId()
        {
            string? loginProividerUserId = null;

            if (TryGetAuthTokenDataFromCookie(out TokenData? tokenData))
            {
                IEnumerable<Claim> claims = ExtractClaimsFromToken(tokenData!.AccessToken);
                loginProividerUserId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            }

            return loginProividerUserId;
        }

        public void DeleteAuthCookies()
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext!;
            httpContext.Response.Cookies.Delete(_configuration["Auth:AuthCookieName"]!);
            httpContext.Response.Cookies.Delete(_configuration["Auth:CsrfCookieName"]!);
        }

        /// <summary>
        /// Determines whether the specified JWT is expired.
        /// </summary>
        /// <param name="jwt">The JSON Web Token (JWT) to check.</param>
        /// <returns><c>true</c> if the JWT has expired; otherwise, <c>false</c>.</returns>
        private bool IsJwtExpired(string jwt)
        {
            try
            {
                JwtSecurityTokenHandler handler = new();
                JwtSecurityToken token = handler.ReadJwtToken(jwt);
                string? expClaim = token.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

                if (string.IsNullOrEmpty(expClaim))
                {
                    return true;
                }

                if (long.TryParse(expClaim, out long expSeconds))
                {
                    DateTimeOffset expiry = DateTimeOffset.FromUnixTimeSeconds(expSeconds);
                    DateTimeOffset now = DateTimeOffset.UtcNow;

                    // Return true if token is expired or will expire within thresholdSeconds
                    return expiry <= now;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing JWT to check expiry.");
                return true;
            }
        }


        /// <summary>
        /// Extracts the claims from a JWT token.
        /// </summary>
        /// <param name="jwt">The JSON Web Token (JWT) string.</param>
        /// <returns>An enumerable of <see cref="Claim"/> objects extracted from the token.</returns>
        private static IEnumerable<Claim> ExtractClaimsFromToken(string jwt)
        {
            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken token = handler.ReadJwtToken(jwt);

            return token.Claims;
        }

        /// <summary>
        /// Attempts to retrieve the CSRF token from the CSRF cookie.
        /// </summary>
        /// <param name="csrfToken">When this method returns, contains the CSRF token if found; otherwise, an empty string.</param>
        /// <returns><c>true</c> if the CSRF token was successfully retrieved; otherwise, <c>false</c>.</returns>
        private bool TryGetCsrfTokenFromCookie(out string? csrfToken)
        {
            csrfToken = string.Empty;
            HttpContext httpContext = _httpContextAccessor.HttpContext!;
            if (httpContext == null)
            {
                return false;
            }

            string cookieName = _configuration["Auth:CsrfCookieName"]!;
            if (!httpContext.Request.Cookies.TryGetValue(cookieName, out string? cookieValue)
                || string.IsNullOrWhiteSpace(cookieValue))
            {
                return false;
            }

            csrfToken = cookieValue;
            return true;
        }
    }
}
