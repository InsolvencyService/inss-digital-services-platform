using INSS.Platform.Auth.API.Dto;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Defines authentication-related operations for the platform.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Generates the login redirect URL for the authentication provider.
        /// </summary>
        /// <param name="clientUrl">The client application's URL to redirect back after authentication.</param>
        /// <param name="userId">The unique identifier of the user initiating the login.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the login redirect URL as a string.
        /// </returns>
        Task<string> GetLoginRedirectUrlAsync(string clientUrl, string userId);

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
        /// Logs out the user using the provided ID token.
        /// </summary>
        /// <param name="idToken">The ID token of the user to log out.</param>
        /// <returns>
        /// A task that represents the asynchronous logout operation.
        /// </returns>
        Task LogOutAsync(string idToken);

        /// <summary>
        /// Determines whether the current user is authenticated.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains <c>true</c> if the user is authenticated; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> IsAuthenticatedAsync();

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
