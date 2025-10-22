using INSS.Platform.Common.Auth.Contracts.Request;
using INSS.Platform.Common.Auth.Contracts.Response;

namespace INSS.Platform.UserManagement.Web.Components.Helpers
{
    /// <summary>
    /// Provides helper methods for authentication operations such as login, logout, token management, and CSRF protection.
    /// </summary>
    public interface IAuthenticationHelper
    {
        /// <summary>
        /// Asynchronously gets the login URL for the authentication provider based on the specified login request.
        /// </summary>
        /// <param name="loginRequest">The login request containing user and client information.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the login URL as a string.</returns>
        Task<string> GetLoginUrlAsync(LoginRequest loginRequest);

        /// <summary>
        /// Asynchronously performs the login operation using the specified login request.
        /// </summary>
        /// <param name="loginRequest">The login request containing user and client information.</param>
        /// <returns>A task that represents the asynchronous login operation.</returns>
        Task LoginAsync(LoginRequest loginRequest);

        /// <summary>
        /// Asynchronously logs out the user using the specified logout request.
        /// </summary>
        /// <param name="logoutRequest">The logout request containing the ID token to be invalidated.</param>
        /// <returns>A task that represents the asynchronous logout operation.</returns>
        Task LogoutAsync(LogoutRequest logoutRequest);

        /// <summary>
        /// Determines whether the current user is signed in.
        /// </summary>
        /// <returns><c>true</c> if the user is signed in; otherwise, <c>false</c>.</returns>
        bool IsUserSignedIn();

        /// <summary>
        /// Persists authentication token data in a cookie for the specified root URI.
        /// </summary>
        /// <param name="rootUri">The root URI for which the authentication token data should be persisted.</param>
        void PersistAuthTokenDataInCookie(Uri rootUri);

        /// <summary>
        /// Attempts to retrieve authentication token data from a cookie.
        /// </summary>
        /// <param name="tokenData">When this method returns, contains the token data if found; otherwise, null.</param>
        /// <returns><c>true</c> if the token data was successfully retrieved; otherwise, <c>false</c>.</returns>
        bool TryGetAuthTokenDataFromCookie(out TokenData? tokenData);

        /// <summary>
        /// Creates a new CSRF token, persists it in a cookie, and returns the token value.
        /// </summary>
        /// <returns>The generated CSRF token as a string.</returns>
        string CreateAndPersistCsrfTokenInCookie();

        /// <summary>
        /// Validates the specified CSRF token against the value stored in the cookie.
        /// </summary>
        /// <param name="csrfToken">The CSRF token to validate.</param>
        /// <returns><c>true</c> if the CSRF token is valid; otherwise, <c>false</c>.</returns>
        bool ValidateCsrfTokenInCookie(string csrfToken);

        /// <summary>
        /// Gets the login provider user ID from the access token claims.
        /// </summary>
        /// <returns>The user ID as provided by the authentication provider.</returns>
        string? GetLoginProviderUserId();
    }
}
