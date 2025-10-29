using INSS.Platform.Auth.Contracts.Request;
using INSS.Platform.Auth.Contracts.Response;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Defines authentication-related operations for the platform.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Generates the login redirect URL for the authentication provider based on the specified login request.
        /// </summary>
        /// <param name="loginRequest">The login request containing CSRF token, client URL, and user ID.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the login redirect URL as a string.
        /// </returns>
        Task<string> GetLoginRedirectUrlAsync(LoginRequest loginRequest);

        /// <summary>
        /// Logs out the user from the authentication provider using the specified logout request.
        /// </summary>
        /// <param name="logoutRequest">The logout request containing necessary information to perform logout.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains <c>true</c> if logout was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> LogoutAsync(LogoutRequest logoutRequest);

        /// <summary>
        /// Handles the authentication callback by exchanging the authorization code for tokens.
        /// </summary>
        /// <param name="authorizationCode">The authorization code received from the authentication provider.</param>
        /// <param name="nonce">The nonce value used to associate a client session with an ID token.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the token data returned from the authentication provider.
        /// </returns>
        Task<TokenData> HandleCallbackAsync(string authorizationCode, string nonce);

        /// <summary>
        /// Validates the state token and extracts authentication parameters.
        /// </summary>
        /// <param name="token">The state token to validate.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a tuple with validation status, nonce, CSRF token, user ID, and client URL.
        /// </returns>
        Task<(bool isValid, string nonce, string csrfToken, string userId, string clientUrl)> ValidateAndExtractRequestStateAsync(string token);
    }
}
