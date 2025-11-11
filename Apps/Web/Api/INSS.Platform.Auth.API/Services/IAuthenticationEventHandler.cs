using INSS.Platform.Auth.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Defines methods for handling authentication events in the authentication process.
    /// </summary>
    public interface IAuthenticationEventHandler
    {
        /// <summary>
        /// Handles the event when an authorization code is received during the authentication process.
        /// </summary>
        /// <param name="context">The context containing information about the authorization code received event.</param>
        /// <param name="provider">The authentication provider associated with the event.</param>
        void HandleAuthorizationCodeReceived(
            AuthorizationCodeReceivedContext context,
            AuthenticationProvider provider);

        /// <summary>
        /// Handles the event when a token is validated during the authentication process.
        /// </summary>
        /// <param name="context">The context containing information about the token validated event.</param>
        /// <param name="provider">The authentication provider associated with the event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleTokenValidatedAsync(
            TokenValidatedContext context,
            AuthenticationProvider provider);

        /// <summary>
        /// Handles the event when redirecting to the identity provider for sign-out.
        /// </summary>
        /// <param name="context">The context containing information about the redirect event.</param>
        /// <param name="signOutCallbackPath">The callback path to use after sign-out.</param>
        /// <param name="provider">The authentication provider associated with the event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleRedirectToIdentityProviderForSignOutAsync(
            RedirectContext context,
            string signOutCallbackPath,
            AuthenticationProvider provider);

        /// <summary>
        /// Handles the event when a remote authentication failure occurs.
        /// </summary>
        /// <param name="context">The context containing information about the remote failure event.</param>
        /// <param name="provider">The authentication provider associated with the event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task HandleRemoteFailureAsync(RemoteFailureContext context,
            AuthenticationProvider provider);
    }
}