using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace INSS.Platform.Auth.API.Services
{
    /// <summary>
    /// Defines methods for handling OpenID Connect authentication events within the platform.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Handles the event when an authorization code is received during the OpenID Connect authentication flow.
        /// </summary>
        /// <param name="context">The context containing information about the authorization code received event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AuthorizationCodeReceivedAsync(AuthorizationCodeReceivedContext context);

        /// <summary>
        /// Handles the event when a token is validated during the OpenID Connect authentication flow.
        /// </summary>
        /// <param name="context">The context containing information about the token validated event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task TokenValidatedAsync(TokenValidatedContext context);

        /// <summary>
        /// Handles the event when redirecting to the identity provider for sign-out during the OpenID Connect authentication flow.
        /// </summary>
        /// <param name="context">The context containing information about the redirect to identity provider event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RedirectToIdentityProviderForSignOutAsync(RedirectContext context);

        /// <summary>
        /// Handles the event when a remote authentication failure occurs during the OpenID Connect authentication flow.
        /// </summary>
        /// <param name="context">The context containing information about the remote failure event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RemoteFailureAsync(RemoteFailureContext context);
    }
}
