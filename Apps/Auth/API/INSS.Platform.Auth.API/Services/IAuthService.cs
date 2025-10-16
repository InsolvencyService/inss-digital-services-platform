using INSS.Platform.Auth.API.Dto;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Defines authentication-related operations for the platform.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Generates the login redirect URL for the authentication provider using the specified state cache key.
        /// </summary>
        /// <param name="stateCacheKey">The unique key used to track authentication state.</param>
        /// <returns>
        /// The URL to redirect the user to the authentication provider's login page.
        /// </returns>
        string GetLoginRedirectUrl(string stateCacheKey);

        /// <summary>
        /// Handles the authentication callback using the provided authorization code and state.
        /// </summary>
        /// <param name="authorizationCode">The authorization code received from the authentication provider.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains <c>true</c> if authentication was successful; otherwise, <c>false</c>.
        /// </returns>
        Task<TokenData> HandleCallbackAsync(string authorizationCode);

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
    }
}
