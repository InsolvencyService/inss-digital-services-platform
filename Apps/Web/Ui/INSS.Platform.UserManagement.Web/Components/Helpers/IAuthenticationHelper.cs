using INSS.Platform.Auth.Contracts.Response;

namespace INSS.Platform.UserManagement.Web.Components.Helpers
{
    /// <summary>
    /// Provides helper methods for authentication operations such as login, logout, token management, and CSRF protection.
    /// </summary>
    public interface IAuthenticationHelper
    {
        /// <summary>
        /// Determines whether the current user is signed in.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the user is signed in; otherwise, <c>false</c>.
        /// </returns>
        bool IsUserSignedIn();

        /// <summary>
        /// Determines whether a JWT exists but has expired.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a JWT exists but is expired; otherwise, <c>false</c>.
        /// </returns>
        bool JwtExistsButHasExpired();

        /// <summary>
        /// Persists authentication token data in a cookie for the specified root URI.
        /// </summary>
        /// <param name="rootUri">The root URI for which the authentication token data should be persisted.</param>
        void PersistAuthTokenDataInCookie(Uri rootUri);

        /// <summary>
        /// Gets the login provider user ID from the access token claims.
        /// </summary>
        /// <returns>
        /// The user ID as provided by the authentication provider, or <c>null</c> if not available.
        /// </returns>
        string? GetLoginProviderUserId();

        /// <summary>
        /// Deletes all authentication-related cookies from the user's browser.
        /// </summary>
        void DeleteAuthCookies();
    }
}
